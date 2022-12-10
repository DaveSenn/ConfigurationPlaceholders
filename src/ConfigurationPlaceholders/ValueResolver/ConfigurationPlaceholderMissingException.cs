namespace ConfigurationPlaceholders;

public sealed class ConfigurationPlaceholderMissingException : Exception
{
    public ConfigurationPlaceholderMissingException( String? message )
        : base( message )
    {
    }
}