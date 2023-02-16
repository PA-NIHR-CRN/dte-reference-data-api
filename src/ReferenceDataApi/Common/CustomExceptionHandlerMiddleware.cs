using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Dte.Common.Exceptions;
using Dte.Common.Extensions;
using Dte.Common.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ReferenceDataApi.Common
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHeaderService _headerService;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IHeaderService headerService, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _headerService = headerService;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            switch (ex)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.Failures);
                    break;
                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { badRequestException.Message});
                    break;
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case UnauthorizedException _:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case InternalServerErrorException internalServerErrorException:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonConvert.SerializeObject(new { internalServerErrorException.Message});
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty)
            {
                result = JsonConvert.SerializeObject(new { error = ex.Message, innerException = ex.InnerException?.Message, conversationId = _headerService.GetConversationId() });
            }

            if (code == HttpStatusCode.BadRequest || code == HttpStatusCode.NotFound || code == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(result);
            }
            else
            {
                _logger.LogError(ex, result);
            }
            
            return context.Response.WriteAsync(result);
        }
    }
}