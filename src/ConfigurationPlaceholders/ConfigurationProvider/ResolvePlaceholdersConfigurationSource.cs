using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

internal sealed class ResolvePlaceholdersConfigurationSource : IConfigurationSource
{
    private readonly IConfigurationRoot? _configuration;
    private readonly MissingPlaceholderValueStrategy _missingPlaceholderValueStrategy;
    private readonly IList<IPlaceholderResolver> _placeholderResolvers;
    private readonly IList<IConfigurationSource>? _sources;

    public ResolvePlaceholdersConfigurationSource( IList<IConfigurationSource> sources,
                                                   IList<IPlaceholderResolver> placeholderResolvers,
                                                   MissingPlaceholderValueStrategy missingPlaceholderValueStrategy )
    {
        _placeholderResolvers = placeholderResolvers;
        _missingPlaceholderValueStrategy = missingPlaceholderValueStrategy;
        _sources = sources;
    }

    public ResolvePlaceholdersConfigurationSource( IConfigurationRoot root,
                                                   IList<IPlaceholderResolver> placeholderResolvers,
                                                   MissingPlaceholderValueStrategy missingPlaceholderValueStrategy )
    {
        _configuration = root;
        _placeholderResolvers = placeholderResolvers;
        _missingPlaceholderValueStrategy = missingPlaceholderValueStrategy;
    }

    public IConfigurationProvider Build( IConfigurationBuilder builder )
    {
        List<IConfigurationProvider> providers;
        if ( _configuration is null )
        {
            providers = new();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( var source in _sources! )
            {
                if ( source is ResolvePlaceholdersConfigurationSource )
                    continue;
                var provider = source.Build( builder );
                providers.Add( provider );
            }
        }
        else
            providers = _configuration.Providers.ToList();

        return new ResolvePlaceholdersConfigurationProvider( new ConfigurationRoot( providers ),
                                                             _placeholderResolvers,
                                                             _missingPlaceholderValueStrategy );
    }
}