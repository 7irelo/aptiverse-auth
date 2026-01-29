using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aptiverse.Auth.Middleware
{
    public class ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger) : IAsyncActionFilter
    {
        private readonly ILogger<ExceptionHandlingFilter> _logger = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                context.Result = new ObjectResult(new
                {
                    Message = "An unexpected error occurred",
                    Error = ex.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
