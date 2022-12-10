using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

/// <summary>
///     Resolves placeholders from environment variables.
///     Searches with this priority:
///     1.  EnvironmentVariableTarget.Process
///     2.  EnvironmentVariableTarget.User
///     3.  EnvironmentVariableTarget.Machine
/// </summary>
public sealed class EnvironmentVariableResolver : IPlaceholderResolver
{
    #region Implementation of IPlaceholderResolver

    /// <summary>
    ///     Gets the value to use instead of the placeholder with the given key.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="key">PPlaceholder key.</param>
    /// <param name="value">Value to use; null if value was not found.</param>
    /// <returns>True if a matching value was found; otherwise, false.</returns>
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