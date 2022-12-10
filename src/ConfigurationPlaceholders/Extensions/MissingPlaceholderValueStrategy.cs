namespace Microsoft.Extensions.Configuration;

public enum MissingPlaceholderValueStrategy
{
    VerifyAllAtStartup = 0,
    Throw = 1,
    UseEmptyValue = 2,
    IgnorePlaceholder = 3
}