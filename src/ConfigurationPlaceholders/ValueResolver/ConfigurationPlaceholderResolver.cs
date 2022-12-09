using Microsoft.Extensions.Configuration;

namespace ConfigurationPlaceholders;

public sealed class ConfigurationPlaceholderResolver : IPlaceholderResolver
{
    #region Implementation of IPlaceholderResolver

    public Boolean GetValue( IConfiguration configuration, String key, out String? value )
    {
        value = configuration[key];
        return value is not null;
    }

    #endregion
}