using System.Reflection;
using ConfigurationPlaceholders;
using Microsoft.Extensions.Configuration;
using Serilog;

var configuration = new ConfigurationBuilder()
                    .AddJsonFile( "appsettings.json" )
                    .AddConfigurationPlaceholders( new List<IPlaceholderResolver>
                    {
                        new InMemoryPlaceholderResolver( new Dictionary<String, String?>
                        {
                            {
                                "ApplicationName", Assembly.GetExecutingAssembly()
                                                           .GetName()
                                                           .Name
                            },
                            {
                                "ApplicationVersion", Assembly.GetExecutingAssembly()
                                                              .GetName()
                                                              .Version!.ToString()
                            },
                            {
                                "Today", DateTime.Today.ToShortDateString()
                            },
                            { "Day", DateTime.Now.DayOfWeek.ToString() }
                        } ),
                        new CallbackPlaceholderResolver( new Dictionary<String, Func<String?>>
                        {
                            { "Time", () => DateTime.Now.ToString( "HH:mm:ss.fff" ) }
                        } ),
                        new ConfigurationPlaceholderResolver(),
                        new EnvironmentVariableResolver()
                    } )
                    .Build();

Environment.SetEnvironmentVariable( "Lookup:DataDir", "C:/Temp/", EnvironmentVariableTarget.Process );

Log.Logger = new LoggerConfiguration()
             .ReadFrom
             .Configuration( configuration )
             .CreateLogger();

try
{
    Log.Logger.Information( "{0}", configuration["Test"] );
    await Task.Delay( 10 );
    Log.Logger.Warning( "{0}", configuration["Test"] );
    Log.Logger.Information( "DB stored here: {0}", configuration["LocalDb"] );
}
catch ( Exception ex )
{
    Log.Logger.Error( ex, "Some error occurred." );
}
finally
{
    Log.CloseAndFlush();
}