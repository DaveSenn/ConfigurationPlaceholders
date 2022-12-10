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
    /// <param name="missingPlaceholderValueHandling">How to handle placeholders with missing values.</param>
    /// <returns><see cref="WebApplicationBuilder" />.</returns>
    public static WebApplicationBuilder AddConfigurationPlaceholders( this WebApplicationBuilder webApplicationBuilder,
                                                                      IList<IPlaceholderResolver> placeholderResolvers,
                                                                      MissingPlaceholderValueHandling missingPlaceholderValueHandling = MissingPlaceholderValueHandling.VerifyAllAtStartup )
    {
        webApplicationBuilder.Configuration.AddConfigurationPlaceholders( placeholderResolvers,
                                                                          missingPlaceholderValueHandling );
        return webApplicationBuilder;
    }

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder" />.</param>
    /// <param name="placeholderResolver">Placeholder value resolver.</param>
    /// <param name="missingPlaceholderValueHandling">How to handle placeholders with missing values.</param>
    /// <returns><see cref="WebApplicationBuilder" />.</returns>
    public static WebApplicationBuilder AddConfigurationPlaceholders( this WebApplicationBuilder webApplicationBuilder,
                                                                      IPlaceholderResolver placeholderResolver,
                                                                      MissingPlaceholderValueHandling missingPlaceholderValueHandling = MissingPlaceholderValueHandling.VerifyAllAtStartup ) =>
        webApplicationBuilder.AddConfigurationPlaceholders( new List<IPlaceholderResolver> { placeholderResolver },
                                                            missingPlaceholderValueHandling );

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="hostBuilder"><see cref="IHostBuilder" />.</param>
    /// <param name="placeholderResolvers">Placeholder value resolvers.</param>
    /// <param name="missingPlaceholderValueHandling">How to handle placeholders with missing values.</param>
    /// <returns><see cref="IHostBuilder" />.</returns>
    public static IHostBuilder AddConfigurationPlaceholders( this IHostBuilder hostBuilder,
                                                             IList<IPlaceholderResolver> placeholderResolvers,
                                                             MissingPlaceholderValueHandling missingPlaceholderValueHandling = MissingPlaceholderValueHandling.VerifyAllAtStartup )
    {
        hostBuilder
            .ConfigureAppConfiguration( ( _, config ) =>
            {
                config.AddConfigurationPlaceholders( placeholderResolvers,
                                                     missingPlaceholderValueHandling );
            } );

        return hostBuilder;
    }

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="hostBuilder"><see cref="IHostBuilder" />.</param>
    /// <param name="placeholderResolver">Placeholder value resolver.</param>
    /// <param name="missingPlaceholderValueHandling">How to handle placeholders with missing values.</param>
    /// <returns><see cref="IHostBuilder" />.</returns>
    public static IHostBuilder AddConfigurationPlaceholders( this IHostBuilder hostBuilder,
                                                             IPlaceholderResolver placeholderResolver,
                                                             MissingPlaceholderValueHandling missingPlaceholderValueHandling = MissingPlaceholderValueHandling.VerifyAllAtStartup ) =>
        hostBuilder.AddConfigurationPlaceholders( new List<IPlaceholderResolver> { placeholderResolver },
                                                  missingPlaceholderValueHandling );

    /// <summary>
    ///     Adds support for placeholders in configuration sources.
    /// </summary>
    /// <param name="configurationBuilder"><see cref="IConfigurationBuilder" />.</param>
    /// <param name="placeholderResolvers">Placeholder value resolvers.</param>
    /// <param name="missingPlaceholderValueHandling">How to handle placeholders with missing values.</param>
    /// <returns><see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddConfigurationPlaceholders( this IConfigurationBuilder configurationBuilder,
                                                                      IList<IPlaceholderResolver> placeholderResolvers,
                                                                      MissingPlaceholderValueHandling missingPlaceholderValueHandling = MissingPlaceholderValueHandling.VerifyAllAtStartup )
    {
        if ( configurationBuilder is IConfigurationRoot configuration )
            configurationBuilder.Add( new ResolvePlaceholdersConfigurationSource( configuration,
                                                                                  placeholderResolvers,
                                                                                  missingPlaceholderValueHandling ) );
        else
        {
            var resolver = new ResolvePlaceholdersConfigurationSource( new List<IConfigurationSource>( configurationBuilder.Sources ),
                                                                       placeholderResolvers,
                                                                       missingPlaceholderValueHandling );
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
    /// <param name="missingPlaceholderValueHandling">How to handle placeholders with missing values.</param>
    /// <returns><see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddConfigurationPlaceholders( this IConfigurationBuilder configurationBuilder,
                                                                      IPlaceholderResolver placeholderResolver,
                                                                      MissingPlaceholderValueHandling missingPlaceholderValueHandling = MissingPlaceholderValueHandling.VerifyAllAtStartup ) =>
        configurationBuilder.AddConfigurationPlaceholders( new List<IPlaceholderResolver> { placeholderResolver },
                                                           missingPlaceholderValueHandling );
}