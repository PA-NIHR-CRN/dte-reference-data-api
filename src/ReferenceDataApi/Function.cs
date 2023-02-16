using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;
using ReferenceDataApi.Extensions;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ReferenceDataApi
{
    public class Function : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>()
                .AddAwsSecrets();
        }

        // Needed for now to be able to deploy locally so the configuration can find the function
        public override Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            return base.FunctionHandlerAsync(request, lambdaContext);
        }
    }
}