using Serilog;
using Serilog.Events;

namespace TravelRequests.Api.Serilog
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
