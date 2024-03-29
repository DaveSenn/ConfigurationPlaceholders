using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationPlaceholders;

internal sealed class ResolvePlaceholdersConfigurationProvider : IConfigurationProvider
{
    private readonly IConfigurationRoot _configuration;
    private readonly MissingPlaceholderValueStrategy _missingPlaceholderValueStrategy;
    private readonly IList<IPlaceholderResolver> _placeholderResolvers;

    public ResolvePlaceholdersConfigurationProvider( IConfigurationRoot root,
                                                     IList<IPlaceholderResolver> placeholderResolvers,
                                                     MissingPlaceholderValueStrategy missingPlaceholderValueStrategy )
    {
        _placeholderResolvers = placeholderResolvers;
        _missingPlaceholderValueStrategy = missingPlaceholderValueStrategy;
        _configuration = root;

        VerifyPlaceholders();
    }

    public IEnumerable<String> GetChildKeys( IEnumerable<String> earlierKeys, String? parentPath )
    {
        IConfiguration section = parentPath is null
            ? _configuration
            : _configuration.GetSection( parentPath );

        var children = section.GetChildren();

        return children.Select( childSection => childSection.Key )
                       .Concat( earlierKeys )
                       .OrderBy( key => key, ConfigurationKeyComparer.Instance );
    }

    public IChangeToken GetReloadToken() =>
        _configuration.GetReloadToken();

    public void Load() =>
        _configuration.Reload();

    public void Set( String key, String? value ) =>
        _configuration[key] = value;

    public Boolean TryGet( String key, out String? value )
    {
        value = _configuration[key];
        if ( value is null )
            return false;

        value = ReplacePlaceholderInValue( value );
        return true;
    }

    private String? ReplacePlaceholderInValue( String? value )
    {
        if ( value is null )
            return value;

        var placeholderStartIndex = 0;
        while ( true )
        {
            if ( placeholderStartIndex >= value.Length )
                return value;

            placeholderStartIndex = value.IndexOf( "${", placeholderStartIndex, StringComparison.Ordinal );
            if ( placeholderStartIndex < 0 )
                return value;

            var placeholderEndIndex = value.IndexOf( '}', placeholderStartIndex );
            var placeholderKey = value.Substring( placeholderStartIndex + 2, placeholderEndIndex - placeholderStartIndex - 2 );

            var placeholderValueProvided = false;
            String? placeholderValue = null;
            for ( var i = _placeholderResolvers.Count - 1; i >= 0; i-- )
            {
                var resolver = _placeholderResolvers[i];
                if ( !resolver.GetValue( _configuration, placeholderKey, out placeholderValue ) )
                    continue;

                placeholderValueProvided = true;
                break;
            }

            if ( !placeholderValueProvided )
                switch ( _missingPlaceholderValueStrategy )
                {
                    case MissingPlaceholderValueStrategy.VerifyAllAtStartup:
                    case MissingPlaceholderValueStrategy.Throw:
                        throw new ConfigurationPlaceholderMissingException( $"Could not resolve a value for placeholder '{placeholderKey}'." );

                    case MissingPlaceholderValueStrategy.UseEmptyValue:
                        placeholderValue = String.Empty;
                        break;

                    case MissingPlaceholderValueStrategy.IgnorePlaceholder:
                        placeholderValue = $"${{{placeholderKey}}}";
                        break;
                }
            else
                placeholderValue = ReplacePlaceholderInValue( placeholderValue );

            value = $"{value[..placeholderStartIndex]}{placeholderValue}{value[( placeholderEndIndex + 1 )..]}";

            placeholderStartIndex++;
        }
    }

    private void VerifyPlaceholders()
    {
        if ( _missingPlaceholderValueStrategy != MissingPlaceholderValueStrategy.VerifyAllAtStartup )
            return;

        foreach ( var (_, value) in _configuration.AsEnumerable() )
        {
            if ( value is null )
                continue;

            ReplacePlaceholderInValue( value );
        }
    }
}