using System.ComponentModel;

[TypeConverter( typeof(TypeConverter<Configuration>) )]
[PublicAPI]
public sealed class Configuration : Enumeration
{
    public static Configuration Debug = new() { Value = nameof(Debug) };
    public static Configuration Release = new() { Value = nameof(Release) };

    public static implicit operator String( Configuration configuration ) =>
        configuration.Value;
}