using System.Text;
using System.Xml;
using Nuke.Common.CI.GitHubActions;

public sealed class Build : NukeBuild
{
    [Solution( GenerateProjects = true )] readonly Solution Solution = default!;

    AbsolutePath ResultDirectory => RootDirectory / "result";
    AbsolutePath ResultNuGetDirectory => ResultDirectory / "nuget";
    AbsolutePath ReSharperSettingsFile => RootDirectory / "data/r#Settings.DotSettings";

    [Parameter]
    Boolean BuildServerOverride { get; }

    Configuration Configuration { get; } = Configuration.Release;

    [Parameter]
    Boolean MasterBranchOverride { get; }

    [GitRepository]
    GitRepository Repository { get; } = default!;

    String Version { get; set; } = "1.0.0";

    Int32 RequiredCoveragePercentage => 95;

    Target CleanBeforeBuild => _ => _
        .Executes( () =>
        {
            EnsureCleanDirectory( ResultDirectory );
        } );

    Target SetVersion => _ => _
        .OnlyWhenDynamic( () => IsServerBuild || BuildServerOverride )
        .DependsOn( CleanBeforeBuild )
        .Executes( () =>
        {
            var version = "1.0.0";

            // Read ls-version
            var versionFile = RootDirectory / "version.json";
            var versionJson = SerializationTasks.JsonDeserializeFromFile( versionFile );
            var lsVersion = versionJson["version"]
                ?.ToString();
            if ( lsVersion is not null )
            {
                version = lsVersion;
                Log.Information( "Version from ls-version: {0}", version );
            }

            var currentVersion = System.Version.Parse( version );
            version = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}.0";
            var assemblyVersion = version;
            var fileVersion = version;

            var isMaster = Repository.Branch!.Equals( "master", StringComparison.OrdinalIgnoreCase );
            if ( !isMaster )
                version = $"{version}-preview-{Repository.Branch}";
            var informationalVersion = $"{version}.{Repository.Commit}";
            Version = version;

            Log.Information( "Version: {0} FileVersion: {1} InformationalVersion: {2}", version, fileVersion, informationalVersion );
            foreach ( var project in Solution.AllProjects )
            {
                var projectModel = ProjectModelTasks.ParseProject( project )!;
                projectModel.SetProperty( "Version", version );
                projectModel.SetProperty( "AssemblyVersion", assemblyVersion );
                projectModel.SetProperty( "FileVersion", fileVersion );
                projectModel.SetProperty( "InformationalVersion", informationalVersion );
                projectModel.Save();
                Log.Information( "SAVE...." );
            }
        } );

    Target Compile => _ => _
        .DependsOn( SetVersion )
        .Executes( () =>
        {
            Log.Information( $"Running build: {Configuration}" );
            DotNetBuild( x => x.SetProjectFile( Solution.Path )
                             .SetConfiguration( Configuration ) );
        } );

    Target Test => _ => _
        .DependsOn( Compile )
        .OnlyWhenDynamic( () => Repository.IsOnMainOrMasterBranch() || MasterBranchOverride )
        .Executes( () =>
        {
            return; // TODO
            DotNetTest( x => x.SetProjectFile( Solution )
                            .SetConfiguration( Configuration )
                            .EnableNoRestore()
                            .EnableNoBuild() );
        } );

    Target TestWithCoverage => _ => _
        .DependsOn( Compile )
        .OnlyWhenDynamic( () => !Repository.IsOnMainOrMasterBranch() && !MasterBranchOverride )
        .Executes( () =>
        {
            return; // TODO
            var dotCover = GetPackageExecutable( "JetBrains.dotCover.CommandLineTools", "dotCover.exe" );
            Log.Information( "Use dotCover: {0}", dotCover );

            var coverageFilters = new HashSet<String>
            {
                "+:ConfigurationPlaceholders",
                "+:ConfigurationPlaceholders.*",
                "-:ConfigurationPlaceholders.Test",
                "-:NukeBuild"
            };
            var attributeFilters = new HashSet<String>
            {
                "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute",
                "System.CodeDom.Compiler.GeneratedCodeAttribute"
            };
            var coverageFiltersString = String.Join( ';', coverageFilters );
            var attributeFiltersString = String.Join( ';', attributeFilters );

            var dotCoverOutputFileName = ResultDirectory / $"{Solution.Name}.dotCover.dcvr";
            var dotCoverOutputFileNameString = dotCoverOutputFileName.ToString()
                .Replace( "\\", "/" );
            var solutionName = Solution.Path!.ToString()
                .Replace( "\\", "/" );

            var sbCmdArgs = new StringBuilder();
            sbCmdArgs.Append( "cover-dotnet " );
            sbCmdArgs.Append( (String?) $"--output=\"{dotCoverOutputFileNameString}\" " );
            sbCmdArgs.Append( (String?) $"--AttributeFilters=\"{attributeFiltersString}\" " );
            sbCmdArgs.Append( (String?) $"--Filters=\"{coverageFiltersString}\" " );
            sbCmdArgs.Append( (String?) $"-- test \"{solutionName}\" --no-build --no-restore --configuration {Configuration} --blame-hang-timeout 1m" );
            var cmdArgs = sbCmdArgs.ToString();

            using var process = StartProcess( dotCover, cmdArgs );
            process.AssertZeroExitCode();

            // Export to TC
            if ( Host is TeamCity )
                TeamCity.Instance.ImportData( TeamCityImportType.dotNetCoverage, dotCoverOutputFileName, TeamCityImportTool.dotcover );

            var xmlReportFileName = ResultDirectory / $"{dotCoverOutputFileName.NameWithoutExtension}.xml";
            DotCoverReport( _ => new List<DotCoverReportSettings>
            {
                new DotCoverReportSettings()
                    .SetOutputFile( xmlReportFileName )
                    .SetReportType( DotCoverReportType.Xml )
                    .SetSource( dotCoverOutputFileName )
            }, Environment.ProcessorCount );

            // Open the XML report
            var doc = new XmlDocument();
            doc.Load( xmlReportFileName.ToString() );

            // Get the root element containing the overall coverage
            var rootElement = doc.SelectSingleNode( "/Root" )!;
            var totalCoverage = Double.Parse( rootElement.Attributes!["CoveragePercent"]!.Value );
            Log.Information( $"Total unit test coverage is: {totalCoverage}%" );

            // Search for uncovered types
            var typesWithoutCoverage = doc.SelectNodes( "//Type[@CoveragePercent='0']" )!;
            if ( typesWithoutCoverage.Count > 0 )
            {
                Log.Information( "Project contains types without coverage:" );
                var uncoveredTypes = new List<String>();
                foreach ( XmlNode type in typesWithoutCoverage )
                {
                    var typeName = type.Attributes!["Name"]!
                        .Value;
                    Log.Error( $"\t{typeName} is not covered by any test" );

                    uncoveredTypes.Add( typeName );
                }

                throw new($"The build contains uncovered types '{String.Join( ",", uncoveredTypes )}'");
            }

            Log.Information( "Code coverage is {0}%", totalCoverage );
            // Check coverage is not too low
            if ( totalCoverage < RequiredCoveragePercentage )
                throw new($"Unit test coverage is too low (must be at least {RequiredCoveragePercentage}% but is only {totalCoverage}%).");
        } );

    Target ScanForVulnerabilities => _ => _
        .OnlyWhenDynamic( () => !Repository.IsOnMainOrMasterBranch() && !MasterBranchOverride )
        .DependsOn( Compile )
        .Executes( () =>
        {
            return; // TODO
            using var process = StartProcess( "dotnet", "list package --vulnerable --include-transitive --source https://api.nuget.org/v3/index.json" );
            process.AssertZeroExitCode();

            var hasErrors = false;
            foreach ( var x in process.Output )
                hasErrors = x.Text.Contains( "has the following vulnerable packages", StringComparison.OrdinalIgnoreCase ) || hasErrors;

            if ( hasErrors )
            {
                foreach ( var x in process.Output )
                    Log.Error( x.Text );
                throw new("Found vulnerable packages.");
            }
        } );

    Target Analyze => _ => _
        .OnlyWhenDynamic( () => !Repository.IsOnMainOrMasterBranch() && !MasterBranchOverride )
        .DependsOn( Test, TestWithCoverage, ScanForVulnerabilities )
        .Executes( () =>
        {
            return; // TODO
            var inspectCode = GetPackageExecutable( "JetBrains.ReSharper.CommandLineTools", "inspectCode.exe" );

            var outputFileName = ResultDirectory / $"{Solution.Name}.InspectionResult.xml";
            var outputFileNameString = outputFileName.ToString()
                .Replace( "\\", "/" );

            var reSharperSettings = ReSharperSettingsFile.ToString()
                .Replace( "\\", "/" );

            var solutionName = Solution.Path!.ToString()
                .Replace( "\\", "/" );

            var sbCmdArgs = new StringBuilder();
            sbCmdArgs.Append( $"/output=\"{outputFileNameString}\" " );
            sbCmdArgs.Append( "/swea " );
            sbCmdArgs.Append( $"/properties:\"configuration={Configuration}\" " );
            sbCmdArgs.Append( $"/profile=\"{reSharperSettings}\" " );
            sbCmdArgs.Append( $"--build \"{solutionName}\"" );
            var cmdArgs = sbCmdArgs.ToString();

            using var process = StartProcess( inspectCode, cmdArgs );
            process.AssertZeroExitCode();
        } );

    Target PrepareNuGetPublish => _ => _
        .DependsOn( Analyze )
        .Produces( ResultNuGetDirectory / "*.nupkg" )
        .Executes( () =>
        {
            Log.Information( "Start packing '{0}'", Solution.src.ConfigurationPlaceholders.Name );
            DotNetPack( x => x.SetProject<DotNetPackSettings>( Solution.src.ConfigurationPlaceholders )
                            .SetConfiguration( Configuration )
                            .EnableNoBuild()
                            .EnableNoRestore()
                            .SetNoDependencies( true )
                            .SetIncludeSource( true )
                            .SetIncludeSymbols( true )
                            .SetSymbolPackageFormat( DotNetSymbolPackageFormat.snupkg )
                            .SetOutputDirectory( ResultNuGetDirectory ) );
        } );

    
    string GithubNugetFeed => GitHubActions != null
        ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
        : null;
    static GitHubActions GitHubActions => GitHubActions.Instance;

    Target PublishNuGetPackage => _ => _
        .DependsOn( PrepareNuGetPublish )
        .OnlyWhenDynamic( () => IsServerBuild || BuildServerOverride )
        .Executes( () =>
        {
            GlobFiles( (String) ResultNuGetDirectory, "*.nupkg" )
                .ForEach( x =>
                {
                    Log.Information( "Start publishing package '{0}' to NuGet.org", x );
                    
                    DotNetNuGetPush(s => s
                                        .SetTargetPath(x)
                                        .SetSource(GithubNugetFeed)
                                        .SetApiKey(GitHubActions.Token)
                                        .EnableSkipDuplicate() );
                    
                 
                    
                } );
            // https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry
            // dotnet nuget add source --username USERNAME --password ${{ secrets.GITHUB_TOKEN }} --store-password-in-clear-text --name github "https://nuget.pkg.github.com/OWNER/index.json"
            // dotnet nuget push "bin/Release/OctocatApp.1.0.0.nupkg" --source "github"
        } );

    Target CreateAndPushGitTag => _ => _
        .OnlyWhenDynamic( () => IsServerBuild || BuildServerOverride )
        .OnlyWhenDynamic( () => Repository.IsOnMainOrMasterBranch() || MasterBranchOverride )
        .DependsOn( PublishNuGetPackage )
        .Executes( () =>
        {
            Git( $"tag {Version}-{DateTime.Now:yyyy-MM-dd-HH:mm:ss}-release" );
            Git( "push --tags" );
        } );

    Target Default => _ => _
        .DependsOn( CreateAndPushGitTag )
        .Executes( () =>
        {
            Log.Information( "Build completed!" );
        } );

    public static Int32 Main() =>
        Execute<Build>( x => x.Default );
}