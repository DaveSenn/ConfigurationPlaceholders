using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders.Test.ValueResolver;

public sealed class ConfigurationPlaceholderResolverTest
{
    [Fact]
    public void GetValue()
    {
        var configuration = new Mock<IConfiguration>();
        configuration
            .SetupGet( x => x["Key1"] )
            .Returns( () => "ValueA" );
        configuration
            .SetupGet( x => x["Key2"] )
            .Returns( () => "ValueB" );

        var target = new ConfigurationPlaceholderResolver();

        var actual = target.GetValue( configuration.Object, "Key1", out var value );
        Assert.True( actual );
        Assert.Equal( "ValueA", value );

        actual = target.GetValue( configuration.Object, "Key2", out value );
        Assert.True( actual );
        Assert.Equal( "ValueB", value );

        actual = target.GetValue( configuration.Object, "missing", out value );
        Assert.False( actual );
        Assert.Null( value );
    }
}