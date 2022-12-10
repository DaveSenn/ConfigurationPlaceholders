namespace ConfigurationPlaceholders;

public sealed class ConfigurationPlaceholderMissing : Exception
{
    public ConfigurationPlaceholderMissing( String? message )
        : base( message )
    {
    }
}