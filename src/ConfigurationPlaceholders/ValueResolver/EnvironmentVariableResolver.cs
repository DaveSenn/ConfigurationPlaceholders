using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

public sealed class EnvironmentVariableResolver : IPlaceholderResolver
{
    #region Implementation of IPlaceholderResolver

    public Boolean GetValue( IConfiguration configuration, String key, out String? value )
    {
        value = Environment.GetEnvironmentVariable( key, EnvironmentVariableTarget.Process );
        if ( value is not null )
            return true;

        value = Environment.GetEnvironmentVariable( key, EnvironmentVariableTarget.User );
        if ( value is not null )
            return true;

        value = Environment.GetEnvironmentVariable( key, EnvironmentVariableTarget.Machine );
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if ( value is not null )
            return true;

        return false;
    }

    #endregion
}