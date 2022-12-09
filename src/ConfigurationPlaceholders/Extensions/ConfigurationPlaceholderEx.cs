using ConfigurationPlaceholders;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Configuration;

/// <summary>
///     Extensions for adding placeholders support.
/// </summary>
public static class ConfigurationPlaceholderEx
{
    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder" />.</param>
    /// <param name="placeholderResolvers">Placeholder value resolvers.</param>
    /// <returns><see cref="WebApplicationBuilder" />.</returns>
    public static WebApplicationBuilder AddConfigurationPlaceholders( this WebApplicationBuilder webApplicationBuilder,
                                                                      IList<IPlaceholderResolver> placeholderResolvers )
    {
        webApplicationBuilder.Configuration.AddConfigurationPlaceholders( placeholderResolvers );
        return webApplicationBuilder;
    }

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder" />.</param>
    /// <param name="placeholderResolver">Placeholder value resolver.</param>
    /// <returns><see cref="WebApplicationBuilder" />.</returns>
    public static WebApplicationBuilder AddConfigurationPlaceholders( this WebApplicationBuilder webApplicationBuilder,
                                                                      IPlaceholderResolver placeholderResolver ) =>
        webApplicationBuilder.AddConfigurationPlaceholders( new List<IPlaceholderResolver> { placeholderResolver } );

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="hostBuilder"><see cref="IHostBuilder" />.</param>
    /// <param name="placeholderResolvers">Placeholder value resolvers.</param>
    /// <returns><see cref="IHostBuilder" />.</returns>
    public static IHostBuilder AddConfigurationPlaceholders( this IHostBuilder hostBuilder,
                                                             IList<IPlaceholderResolver> placeholderResolvers )
    {
        hostBuilder
            .ConfigureAppConfiguration( ( _, config ) => { config.AddConfigurationPlaceholders( placeholderResolvers ); } );

        return hostBuilder;
    }

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="hostBuilder"><see cref="IHostBuilder" />.</param>
    /// <param name="placeholderResolver">Placeholder value resolver.</param>
    /// <returns><see cref="IHostBuilder" />.</returns>
    public static IHostBuilder AddConfigurationPlaceholders( this IHostBuilder hostBuilder,
                                                             IPlaceholderResolver placeholderResolver ) =>
        hostBuilder.AddConfigurationPlaceholders( new List<IPlaceholderResolver> { placeholderResolver } );

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="configurationBuilder"><see cref="IConfigurationBuilder" />.</param>
    /// <param name="placeholderResolvers">Placeholder value resolvers.</param>
    /// <returns><see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddConfigurationPlaceholders( this IConfigurationBuilder configurationBuilder,
                                                                      IList<IPlaceholderResolver> placeholderResolvers )
    {
        if ( configurationBuilder is IConfigurationRoot configuration )
            configurationBuilder.Add( new ResolvePlaceholdersConfigurationSource( configuration, placeholderResolvers ) );
        else
        {
            var resolver = new ResolvePlaceholdersConfigurationSource( new List<IConfigurationSource>( configurationBuilder.Sources ), placeholderResolvers );
            configurationBuilder.Sources.Clear();
            configurationBuilder.Add( resolver );
        }

        return configurationBuilder;
    }

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="configurationBuilder"><see cref="IConfigurationBuilder" />.</param>
    /// <param name="placeholderResolver">Placeholder value resolver.</param>
    /// <returns><see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddConfigurationPlaceholders( this IConfigurationBuilder configurationBuilder,
                                                                      IPlaceholderResolver placeholderResolver ) =>
        configurationBuilder.AddConfigurationPlaceholders( new List<IPlaceholderResolver> { placeholderResolver } );
}