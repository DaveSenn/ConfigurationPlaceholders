using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationPlaceholders.Test.ValueResolver;

public sealed class ResolvePlaceholdersConfigurationProviderTest
{
    [Fact]
    public void GetChildKeys()
    {
        var expectedConfiguration = new Dictionary<String, String?>
        {
            { "A", "Va" },
            { "A:B", "Vb" },
            { "A:B:C", "Vc" },
            { "A:D", "Vd" }
        };

        var configurationRoot = new ConfigurationBuilder()
                                .AddInMemoryCollection( expectedConfiguration )
                                .Build();

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();

        var target = new ResolvePlaceholdersConfigurationProvider( configurationRoot,
                                                                   new List<IPlaceholderResolver>
                                                                   {
                                                                       placeholderResolverMock0.Object
                                                                   } );

        var actual = target.GetChildKeys( Array.Empty<String>(), "A" )
                           .ToList();
        Assert.Equal( 2, actual.Count );
        Assert.Contains( "B", actual );
        Assert.Contains( "D", actual );
    }

    [Fact]
    public void GetChildKeys_EarlierKeys()
    {
        var expectedConfiguration = new Dictionary<String, String?>
        {
            { "A", "Va" },
            { "A:B", "Vb" },
            { "A:B:C", "Vc" },
            { "A:D", "Vd" }
        };

        var configurationRoot = new ConfigurationBuilder()
                                .AddInMemoryCollection( expectedConfiguration )
                                .Build();

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();

        var target = new ResolvePlaceholdersConfigurationProvider( configurationRoot,
                                                                   new List<IPlaceholderResolver>
                                                                   {
                                                                       placeholderResolverMock0.Object
                                                                   } );

        var actual = target.GetChildKeys( new[] { "A" }, "A" )
                           .ToList();
        Assert.Equal( 3, actual.Count );
        Assert.Contains( "A", actual );
        Assert.Contains( "B", actual );
        Assert.Contains( "D", actual );
    }

    [Fact]
    public void GetReloadToken()
    {
        var changeTokenMock = new Mock<IChangeToken>();

        var configurationRootMock = new Mock<IConfigurationRoot>();
        configurationRootMock
            .Setup( x => x.GetReloadToken() )
            .Returns( () => changeTokenMock.Object );

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();
        var placeholderResolverMock1 = new Mock<IPlaceholderResolver>();

        var target = new ResolvePlaceholdersConfigurationProvider( configurationRootMock.Object,
                                                                   new List<IPlaceholderResolver>
                                                                   {
                                                                       placeholderResolverMock0.Object,
                                                                       placeholderResolverMock1.Object
                                                                   } );

        var actual = target.GetReloadToken();
        Assert.Same( changeTokenMock.Object, actual );
    }

    [Fact]
    public void Load()
    {
        var configurationRootMock = new Mock<IConfigurationRoot>();

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();
        var placeholderResolverMock1 = new Mock<IPlaceholderResolver>();

        var target = new ResolvePlaceholdersConfigurationProvider( configurationRootMock.Object,
                                                                   new List<IPlaceholderResolver>
                                                                   {
                                                                       placeholderResolverMock0.Object,
                                                                       placeholderResolverMock1.Object
                                                                   } );

        target.Load();

        configurationRootMock
            .Verify( x => x.Reload(), Times.Once );
    }

    [Fact]
    public void Set()
    {
        var configurationRootMock = new Mock<IConfigurationRoot>();
        configurationRootMock
            .SetupSet( x => x["a"] = "newValue" )
            .Verifiable();

        var placeholderResolverMock0 = new Mock<IPlaceholderResolver>();
        var placeholderResolverMock1 = new Mock<IPlaceholderResolver>();

        var target = new ResolvePlaceholdersConfigurationProvider( configurationRootMock.Object,
                                                                   new List<IPlaceholderResolver>
                                                                   {
                                                                       placeholderResolverMock0.Object,
                                                                       placeholderResolverMock1.Object
                                                                   } );

        target.Set( "a", "newValue" );

        configurationRootMock.Verify();
    }

    [Fact]
    public void TryGet()
    {
        var expectedConfiguration = new Dictionary<String, String?>
        {
            { "A", "NoPlaceholder" },
            { "A:B", "NoPlaceholderB" },
            { "Simple", "${Key1}" },
            { "WithText", "Start-${Key1}-End" },
            { "Multiple", "Start-${Key1}-End ${Key2} ${Key3} ${MissingKey1} ${Key4} ${MissingKey2}" }
        };

        var configurationRoot = new ConfigurationBuilder()
                                .AddInMemoryCollection( expectedConfiguration )
                                .Build();

        var placeholderResolverMock0 = new InMemoryPlaceholderResolver( new Dictionary<String, String?>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            { "Key4", "Value4" }
        } );
        var placeholderResolverMock1 = new InMemoryPlaceholderResolver( new Dictionary<String, String?>
        {
            { "Key1", "Value1.1" },
            { "Key3", "Value3" }
        } );

        var target = new ResolvePlaceholdersConfigurationProvider( configurationRoot,
                                                                   new List<IPlaceholderResolver>
                                                                   {
                                                                       placeholderResolverMock0,
                                                                       placeholderResolverMock1
                                                                   } );

        var actual = target.TryGet( "A", out var value );
        Assert.True( actual );
        Assert.Equal( expectedConfiguration["A"], value );

        actual = target.TryGet( "A:B", out value );
        Assert.True( actual );
        Assert.Equal( expectedConfiguration["A:B"], value );

        actual = target.TryGet( "Missing", out value );
        Assert.False( actual );
        Assert.Null( value );

        actual = target.TryGet( "Simple", out value );
        Assert.True( actual );
        Assert.Equal( "Value1.1", value );

        actual = target.TryGet( "WithText", out value );
        Assert.True( actual );
        Assert.Equal( "Start-Value1.1-End", value );

        actual = target.TryGet( "Multiple", out value );
        Assert.True( actual );
        Assert.Equal( "Start-Value1.1-End Value2 Value3 ${MissingKey1} Value4 ${MissingKey2}", value );
    }
}