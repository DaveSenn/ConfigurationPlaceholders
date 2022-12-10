using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders.Test.ValueResolver;

public sealed class CallbackPlaceholderResolverTest
{
    [Fact]
    public void GetValue()
    {
        var configuration = new Mock<IConfiguration>();

        var values = new Dictionary<String, Func<String?>>
        {
            {
                "a", () => "ValueA"
            },
            {
                "b", () => "ValueB"
            }
        };
        var target = new CallbackPlaceholderResolver( values );

        var actual = target.GetValue( configuration.Object, "a", out var value );
        Assert.True( actual );
        Assert.Equal( values["a"](), value );

        actual = target.GetValue( configuration.Object, "b", out value );
        Assert.True( actual );
        Assert.Equal( values["b"](), value );

        actual = target.GetValue( configuration.Object, "missing", out value );
        Assert.False( actual );
        Assert.Null( value );
    }
}