using Serilog;
using Serilog.Events;

namespace Facets.Api.DIServiceExtensions
{
    public static class SerilogConfig
    {
        public static WebApplicationBuilder AddSerilogConfig(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                   .ReadFrom.Configuration(builder.Configuration)
                   .WriteTo.Console()
                   .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/log-.txt"),
                                 restrictedToMinimumLevel: LogEventLevel.Error,
                                 rollingInterval: RollingInterval.Day)
                   .CreateLogger();
            }
            else
            {
                // On Azure App Service the app folder can be read-only (run from package), so the old
                // file-only logger could write nothing and the portal's Log stream stayed empty.
                // Console output shows up in Log stream / App Service logs; the file goes under
                // %HOME%\LogFiles, which App Service keeps writable and downloadable.
                string logDirectory = Environment.GetEnvironmentVariable("HOME") is { Length: > 0 } home
                    ? Path.Combine(home, "LogFiles", "Facets")
                    : Path.Combine(AppContext.BaseDirectory, "Logs");

                Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Information()
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                   .MinimumLevel.Override("System", LogEventLevel.Warning)
                   .WriteTo.Console()
                   .WriteTo.File(Path.Combine(logDirectory, "log-.txt"),
                                 restrictedToMinimumLevel: LogEventLevel.Warning,
                                 rollingInterval: RollingInterval.Day,
                                 retainedFileCountLimit: 14)
                   .CreateLogger();
            }

            return builder;
        }
    }
}
