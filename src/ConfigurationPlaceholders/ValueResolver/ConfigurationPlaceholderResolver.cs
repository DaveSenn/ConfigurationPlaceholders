using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

/// <summary>
///     Provides placeholder values based on the application configuration.
///     Placeholder key must match an existing configuration entry.
/// </summary>
public sealed class ConfigurationPlaceholderResolver : IPlaceholderResolver
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
        value = configuration[key];
        return value is not null;
    }

    #endregion
}