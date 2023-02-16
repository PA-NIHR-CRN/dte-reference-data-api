using Microsoft.AspNetCore.Builder;
using ReferenceDataApi.Common;

namespace ReferenceDataApi.Extensions
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
        
        public static IApplicationBuilder UseCustomHeaderForwarderHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HeaderForwarderMiddleware>();
        }
    }
}