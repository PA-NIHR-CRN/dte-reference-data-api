using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ReferenceDataApi.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapCustomHealthCheck(
            this IEndpointRouteBuilder endpoints,
            string pattern = "/api/health",
            string servicesPattern = "/api/health/ready")
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            endpoints.MapHealthChecks(pattern, new HealthCheckOptions
            {
                Predicate = check => !check.Tags.Contains("services"),
                AllowCachingResponses = false,
                ResponseWriter = WriteResponse,
            });

            endpoints.MapHealthChecks(servicesPattern, new HealthCheckOptions
            {
                Predicate = check => true,
                AllowCachingResponses = false,
                ResponseWriter = WriteResponse,
            });

            return endpoints;
        }

        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                writer.WriteStartObject();
                writer.WriteString("status", result.Status.ToString());
                writer.WriteStartObject("services");
                foreach (var (key, value) in result.Entries)
                {
                    writer.WriteStartObject(key);
                    writer.WriteString("status", value.Status.ToString());
                    
                    if (!string.IsNullOrWhiteSpace(value.Description))
                    {
                        writer.WriteString("description", value.Description);
                    }
                    
                    foreach (var (dataKey, dataValue) in value.Data)
                    {
                        writer.WriteString(dataKey, dataValue.ToString());
                    }
                    
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            var json = Encoding.UTF8.GetString(stream.ToArray());

            return context.Response.WriteAsync(json);
        }
    }
}