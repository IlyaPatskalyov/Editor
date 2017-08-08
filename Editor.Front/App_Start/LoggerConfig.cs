using Editor.Front.Settings;
using Serilog;
using Serilog.Core;
using SerilogWeb.Classic.Enrichers;

namespace Editor.Front
{
    public static class LoggerConfig
    {
        public static Logger Build()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                 .WriteTo.RollingFile(new ApplicationSettings().LogPath)
                 .Enrich.With<HttpRequestIdEnricher>()
                 .Enrich.With<HttpRequestTraceIdEnricher>()
                 .Enrich.With<UserNameEnricher>()
                 .CreateLogger();
        }
    }
}