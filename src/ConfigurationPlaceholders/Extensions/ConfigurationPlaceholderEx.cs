using ConfigurationPlaceholders;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationPlaceholderEx
{
    public static WebApplicationBuilder AddConfigurationPlaceholders( this WebApplicationBuilder applicationBuilder,
                                                                      IList<IPlaceholderResolver> placeholderResolvers )
    {
        applicationBuilder.Configuration.AddConfigurationPlaceholders( placeholderResolvers );
        return applicationBuilder;
    }

    public static IHostBuilder AddConfigurationPlaceholders( this IHostBuilder hostBuilder,
                                                             IList<IPlaceholderResolver> placeholderResolvers )
    {
        hostBuilder
            .ConfigureAppConfiguration( ( _, config ) => { config.AddConfigurationPlaceholders( placeholderResolvers ); } );

        return hostBuilder;
    }

    public static IConfigurationBuilder AddConfigurationPlaceholders( this IConfigurationBuilder builder,
                                                                      IList<IPlaceholderResolver> placeholderResolvers )
    {
        if ( builder is IConfigurationRoot configuration )
            builder.Add( new ResolvePlaceholdersConfigurationSource( configuration, placeholderResolvers ) );
        else
        {
            var resolver = new ResolvePlaceholdersConfigurationSource( new List<IConfigurationSource>( builder.Sources ), placeholderResolvers );
            builder.Sources.Clear();
            builder.Add( resolver );
        }

        return builder;
    }
}