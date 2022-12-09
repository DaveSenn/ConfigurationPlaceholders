using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

/// <summary>
///     Resolves placeholder values with a in-memory lookup.
/// </summary>
public sealed class InMemoryPlaceholderResolver : IPlaceholderResolver
{
    private readonly IReadOnlyDictionary<String, String?> _values;

    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="values">
    ///     Placeholder values.
    ///     Key => Placeholder key.
    ///     Value => Value to use.
    /// </param>
    public InMemoryPlaceholderResolver( IReadOnlyDictionary<String, String?> values ) =>
        _values = values;

    #region Implementation of IPlaceholderResolver

    /// <summary>
    ///     Gets the value to use instead of the placeholder with the given key.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    /// <param name="key">PPlaceholder key.</param>
    /// <param name="value">Value to use; null if value was not found.</param>
    /// <returns>True if a matching value was found; otherwise, false.</returns>
    public Boolean GetValue( IConfiguration configuration, String key, out String? value ) =>
        _values.TryGetValue( key, out value );

    #endregion
}