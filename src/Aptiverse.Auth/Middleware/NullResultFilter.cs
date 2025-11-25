using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aptiverse.Auth.Middleware
{
    public class NullResultFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            if (result.Result is ObjectResult objectResult && objectResult.Value is null)
            {
                result.Result = new NotFoundResult();
            }
        }
    }
}
