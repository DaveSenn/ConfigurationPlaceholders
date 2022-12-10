using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders.Test.ValueResolver;

public sealed class ResolvePlaceholdersConfigurationSourceTest
{
    [Fact]
    public void Build()
    {
        var expectedConfiguration = new Dictionary<String, String?>
        {
            { "A", "Va" }
        };

        var configurationRoot = new ConfigurationBuilder()
                                .AddInMemoryCollection( expectedConfiguration )
                                .Build();

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();

        var configurationBuilderMock = new Mock<IConfigurationBuilder>();

        var target = new ResolvePlaceholdersConfigurationSource( configurationRoot,
                                                                 new List<IPlaceholderResolver>
                                                                 {
                                                                     placeholderResolverMock0.Object
                                                                 },
                                                                 MissingPlaceholderValueHandling.UseEmptyValue );

        var actual = target.Build( configurationBuilderMock.Object );
        Assert.IsType<ResolvePlaceholdersConfigurationProvider>( actual );

        actual.TryGet( "A", out var value );
        Assert.Equal( "Va", value );
    }

    [Fact]
    public void Build_SourcesOnly()
    {
        var configurationBuilderMock = new Mock<IConfigurationBuilder>();

        var configurationProviderMock0 = new Mock<IConfigurationProvider>();
        var configurationProviderMock1 = new Mock<IConfigurationProvider>();

        var nestedProblem = new ResolvePlaceholdersConfigurationSource( new ConfigurationManager().AddInMemoryCollection()
                                                                                                  .Build(),
                                                                        new List<IPlaceholderResolver>() ,
                                                                        MissingPlaceholderValueHandling.UseEmptyValue);

        var configurationSourceMock0 = new Mock<IConfigurationSource>();
        configurationSourceMock0
            .Setup( x => x.Build( configurationBuilderMock.Object ) )
            .Returns( () => configurationProviderMock0.Object );
        var configurationSourceMock1 = new Mock<IConfigurationSource>();
        configurationSourceMock1
            .Setup( x => x.Build( configurationBuilderMock.Object ) )
            .Returns( () => configurationProviderMock1.Object );

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();

        var target = new ResolvePlaceholdersConfigurationSource( new List<IConfigurationSource>
                                                                 {
                                                                     configurationSourceMock0.Object,
                                                                     configurationSourceMock1.Object,
                                                                     nestedProblem
                                                                 },
                                                                 new List<IPlaceholderResolver>
                                                                 {
                                                                     placeholderResolverMock0.Object
                                                                 } ,
                                                                 MissingPlaceholderValueHandling.UseEmptyValue);

        var actual = target.Build( configurationBuilderMock.Object );
        Assert.IsType<ResolvePlaceholdersConfigurationProvider>( actual );

        configurationSourceMock0
            .Verify( x => x.Build( configurationBuilderMock.Object ), Times.Once );
        configurationSourceMock1
            .Verify( x => x.Build( configurationBuilderMock.Object ), Times.Once );
    }
}