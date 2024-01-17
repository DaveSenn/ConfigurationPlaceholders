using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders.Test.ValueResolver;

public sealed class EnvironmentVariableResolverTest
{
    [Fact]
    public void GetValue()
    {
        var configuration = new Mock<IConfiguration>();

        Environment.SetEnvironmentVariable( "Test:Key1", "A0", EnvironmentVariableTarget.Process );
        Environment.SetEnvironmentVariable( "Test:Key1", "A1", EnvironmentVariableTarget.User );
        Environment.SetEnvironmentVariable( "Test:Key2", "B1", EnvironmentVariableTarget.User );

        try
        {
            var target = new EnvironmentVariableResolver();

            var actual = target.GetValue( configuration.Object, "Test:Key1", out var value );
            Assert.True( actual );
            Assert.Equal( "A0", value );

            actual = target.GetValue( configuration.Object, "Test:Key2", out value );
            Assert.True( actual );
            Assert.Equal( "B1", value );

            actual = target.GetValue( configuration.Object, "Test:Key3", out value );
            Assert.False( actual );
            Assert.Null( value );

            actual = target.GetValue( configuration.Object, "missing", out value );
            Assert.False( actual );
            Assert.Null( value );
        }
        finally
        {
            Environment.SetEnvironmentVariable( "Test:Key1", null, EnvironmentVariableTarget.Process );
            Environment.SetEnvironmentVariable( "Test:Key1", null, EnvironmentVariableTarget.User );
            Environment.SetEnvironmentVariable( "Test:Key2", null, EnvironmentVariableTarget.User );
        }
    }
}