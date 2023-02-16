using System.Linq;
using System.Threading.Tasks;
using Dte.Common.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ReferenceDataApi.Common
{
    public class HeaderForwarderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHeaderService _headerService;
        private readonly ILogger<HeaderForwarderMiddleware> _logger;
        
        private readonly string[] _headersToForward = { "x-dte-version", "Postman-Token" };

        public HeaderForwarderMiddleware(RequestDelegate next, IHeaderService headerService, ILogger<HeaderForwarderMiddleware> logger)
        {
            _next = next;
            _headerService = headerService;
            _logger = logger;
        }
        
        public async Task Invoke(HttpContext context)
        {
            _headerService.ClearHeaders();
            
            foreach (var (key, value) in context.Request.Headers)
            {
                if(!DefaultHeaders.All.Union(_headersToForward).Contains(key)) continue;
                
                _headerService.SetHeader(key, value);   
            }
            
            await _next(context);
        }
    }
}