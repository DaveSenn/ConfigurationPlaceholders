using System.Net.NetworkInformation;
using ConfigurationPlaceholders;

var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
var fullDomainName = ipProperties.HostName.ToLower();
if ( !String.IsNullOrWhiteSpace( ipProperties.DomainName ) )
    fullDomainName = $"{ipProperties.HostName}.{ipProperties.DomainName}".ToLower();

var builder = WebApplication.CreateBuilder( args );
builder
    .AddConfigurationPlaceholders( new InMemoryPlaceholderResolver( new Dictionary<String, String?>
                                   {
                                       { "FQDN", fullDomainName }
                                   } ),
                                   MissingPlaceholderValueStrategy.UseEmptyValue );

var app = builder.Build();
app.MapGet( "/GetCertInfo", ( IConfiguration configuration ) => $"Use certificate with subject {configuration["CertificateSubject"]} for HTTPS connection." );
app.Run();