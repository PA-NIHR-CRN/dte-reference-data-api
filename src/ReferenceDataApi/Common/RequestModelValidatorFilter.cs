using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ReferenceDataApi.Common
{
    public class RequestModelValidatorFilter : IAsyncActionFilter
    {
        private readonly ILogger<RequestModelValidatorFilter> _logger;

        public RequestModelValidatorFilter(ILogger<RequestModelValidatorFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
                var errorsString = string.Join("; ", errors);
                _logger.LogWarning("Request Validation Failed: {Errors}", errorsString);
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            else
            {
                await next();
            }
        }
    }
}
