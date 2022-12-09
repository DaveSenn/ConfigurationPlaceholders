using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

public interface IPlaceholderResolver
{
    Boolean GetValue( IConfiguration configuration, String key, out String? value );
}