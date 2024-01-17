using System.ComponentModel;

[TypeConverter( typeof(TypeConverter<Configuration>) )]
[PublicAPI]
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CA2211 // Non-constant fields should not be visible
public sealed class Configuration : Enumeration
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static Configuration Debug = new() { Value = nameof(Debug) };
    public static Configuration Release = new() { Value = nameof(Release) };

    public static implicit operator String( Configuration configuration ) =>
        configuration.Value;
}
#pragma warning restore CA2211 // Non-constant fields should not be visible