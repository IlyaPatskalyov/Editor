using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Editor.Front.WebApiExtensions
{
    public class LogMessageHandler : DelegatingHandler
    {
        private readonly ILogger logger;

        public LogMessageHandler(ILogger logger)
        {
            this.logger = logger;
        }

        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            logger.Information("HttpMethod Call: {RequestUri}", request.RequestUri);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            HttpResponseMessage response = null;
            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                stopwatch.Stop();

                logger.Information("HttpMethod Finished: {RequestUri}, Time: {RequestTime}, StatusCode: {StatusCode}", request.RequestUri,
                                   stopwatch.ElapsedMilliseconds, response?.StatusCode);
            }
            return response;
        }
    }
}