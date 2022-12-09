namespace ConfigurationPlaceholders;

public sealed class InMemoryPlaceholderResolver : IPlaceholderResolver
{
    private readonly IReadOnlyDictionary<String, String?> _values;

    public InMemoryPlaceholderResolver( IReadOnlyDictionary<String, String?> values ) =>
        _values = values;

    #region Implementation of IPlaceholderResolver

    public Boolean GetValue( String key, out String? value ) =>
        _values.TryGetValue( key, out value );

    #endregion
}