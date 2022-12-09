using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConfigurationPlaceholders.Test.ValueResolver;

public sealed class ConfigurationPlaceholderExTest
{
    [Fact]
    public void AddConfigurationPlaceholders_ConfigurationBuilder()
    {
        var placeholderResolvers = new List<IPlaceholderResolver>
        {
            new InMemoryPlaceholderResolver( new Dictionary<String, String?>
            {
                { "Key1", "MyValue" }
            } )
        };

        IConfigurationBuilder builder = new ConfigurationBuilder();
        builder
            .AddInMemoryCollection( new Dictionary<String, String?>
            {
                { "Name", "Value-${Key1}" }
            } )
            .Build();

        var actual = builder.AddConfigurationPlaceholders( placeholderResolvers );
        Assert.Same( builder, actual );

        var configuration = builder.Build();
        var value = configuration["Name"];
        Assert.Equal( "Value-MyValue", value );
    }

    [Fact]
    public void AddConfigurationPlaceholders_HostBuilder()
    {
        var placeholderResolvers = new List<IPlaceholderResolver>
        {
            new InMemoryPlaceholderResolver( new Dictionary<String, String?>
            {
                { "Key1", "MyValue" }
            } )
        };

        var builder = Host.CreateDefaultBuilder()
                          .ConfigureAppConfiguration( ( _, config ) =>
                          {
                              config.AddInMemoryCollection( new Dictionary<String, String?>
                              {
                                  { "Name", "Value-${Key1}" }
                              } );
                          } );

        var actual = builder.AddConfigurationPlaceholders( placeholderResolvers );
        Assert.Same( builder, actual );

        var host = builder.Build();
        var configuration = host.Services.GetRequiredService<IConfiguration>();

        var value = configuration["Name"];
        Assert.Equal( "Value-MyValue", value );
    }

    [Fact]
    public void AddConfigurationPlaceholders_WebApplicationBuilder()
    {
        var placeholderResolvers = new List<IPlaceholderResolver>
        {
            new InMemoryPlaceholderResolver( new Dictionary<String, String?>
            {
                { "Key1", "MyValue" }
            } )
        };

        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection( new Dictionary<String, String?>
        {
            { "Name", "Value-${Key1}" }
        } );
        var actual = builder.AddConfigurationPlaceholders( placeholderResolvers );

        Assert.Same( builder, actual );

        var value = builder.Configuration["Name"];
        Assert.Equal( "Value-MyValue", value );
    }
}