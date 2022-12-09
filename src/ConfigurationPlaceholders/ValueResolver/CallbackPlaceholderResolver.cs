using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

public sealed class CallbackPlaceholderResolver : IPlaceholderResolver
{
    private readonly IReadOnlyDictionary<String, Func<String?>> _values;

    public CallbackPlaceholderResolver( IReadOnlyDictionary<String, Func<String?>> values ) =>
        _values = values;

    #region Implementation of IPlaceholderResolver

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