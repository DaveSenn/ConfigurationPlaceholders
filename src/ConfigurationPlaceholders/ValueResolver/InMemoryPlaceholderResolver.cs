using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

/// <summary>
///     Resolves placeholder values with a in-memory lookup.
/// </summary>
/// <remarks>
///     Ctor.
/// </remarks>
/// <param name="values">
///     Placeholder values.
///     Key => Placeholder key.
///     Value => Value to use.
/// </param>
public sealed class InMemoryPlaceholderResolver( IReadOnlyDictionary<String, String?> values ) : IPlaceholderResolver
{
    #region Implementation of IPlaceholderResolver

    /// <summary>
    ///     Gets the value to use instead of the placeholder with the given key.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="key">PPlaceholder key.</param>
    /// <param name="value">Value to use; null if value was not found.</param>
    /// <returns>True if a matching value was found; otherwise, false.</returns>
    public Boolean GetValue( IConfiguration configuration, String key, out String? value ) =>
        values.TryGetValue( key, out value );

    #endregion
}