namespace ConfigurationPlaceholders;

public interface IPlaceholderResolver
{
    Boolean GetValue( String key, out String? value );
}