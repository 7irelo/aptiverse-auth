using Microsoft.AspNetCore.Mvc.Filters;

namespace Aptiverse.Auth.Middleware
{
    public class LoggingFilter(ILogger<LoggingFilter> logger) : IAsyncActionFilter
    {
        private readonly ILogger<LoggingFilter> _logger = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionName = context.ActionDescriptor.DisplayName;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Executing action: {ActionName}", actionName);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await next();

            stopwatch.Stop();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Action {ActionName} executed in {ElapsedMilliseconds}ms",
                    actionName, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
