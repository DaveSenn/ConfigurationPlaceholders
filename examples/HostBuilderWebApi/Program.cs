using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ConfigurationPlaceholders;

#pragma warning disable CA1050 // Declare types in namespaces
public static class Program
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static void Main( String[] args )
    {
        CreateHostBuilder( args )
            .Build()
            .Run();
    }

    private static IHostBuilder CreateHostBuilder( String[] args )
    {
        var framework = Assembly
                        .GetEntryAssembly()
                        ?
                        .GetCustomAttribute<TargetFrameworkAttribute>()
                        ?
                        .FrameworkName;

        return Host
               .CreateDefaultBuilder( args )
               .AddConfigurationPlaceholders( new InMemoryPlaceholderResolver( new Dictionary<String, String?>
                                              {
                                                  { "Framework", framework },
                                                  { "OsInfo", RuntimeInformation.OSDescription }
                                              } ),
                                              MissingPlaceholderValueStrategy.IgnorePlaceholder )
               .ConfigureWebHostDefaults( webBuilder => { webBuilder.UseStartup<Startup>(); } );
    }
}