using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

/// <summary>
///     Resolves values of placeholders.
/// </summary>
public interface IPlaceholderResolver
{
    /// <summary>
    ///     Gets the value to use instead of the placeholder with the given key.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="key">PPlaceholder key.</param>
    /// <param name="value">Value to use; null if value was not found.</param>
    /// <returns>True if a matching value was found; otherwise, false.</returns>
    Boolean GetValue( IConfiguration configuration, String key, out String? value );
}