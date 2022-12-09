using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

/// <summary>
///     Resolves placeholder values based on value factories.
/// </summary>
public sealed class CallbackPlaceholderResolver : IPlaceholderResolver
{
    private readonly IReadOnlyDictionary<String, Func<String?>> _values;

    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="values">
    ///     Value factories.
    ///     Key => Placeholder key.
    ///     Value => Value factory.
    /// </param>
    public CallbackPlaceholderResolver( IReadOnlyDictionary<String, Func<String?>> values ) =>
        _values = values;

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
        if ( !_values.TryGetValue( key, out var factory ) )
        {
            value = null;
            return false;
        }

        value = factory();
        return true;
    }

    #endregion
}