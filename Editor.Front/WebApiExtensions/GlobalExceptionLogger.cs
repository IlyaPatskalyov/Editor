using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Serilog;

namespace Editor.Front.WebApiExtensions
{
    public class GlobalExceptionLogger : IExceptionLogger
    {
        private readonly ILogger logger;

        public GlobalExceptionLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            logger.Error(context.Exception, "HttpMethod UnhandledException Url: {RequestUri}", context.Request.RequestUri);
            return Task.Delay(0, cancellationToken);
        }
    }
}