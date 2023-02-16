using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Responses;
using Dte.Common.Helpers;
using MediatR;
using Newtonsoft.Json;

namespace Application.Demographics.V1.Queries
{
    public class GetEthnicityDataQuery : IRequest<Dictionary<string, EthnicityResponse>>
    {
        public GetEthnicityDataQuery()
        {
        }

        public class GetEthnicityDataQueryHandler : IRequestHandler<GetEthnicityDataQuery, Dictionary<string, EthnicityResponse>>
        {
            public GetEthnicityDataQueryHandler()
            {
            }

            public async Task<Dictionary<string, EthnicityResponse>> Handle(GetEthnicityDataQuery request, CancellationToken cancellationToken)
            {
                return JsonConvert.DeserializeObject<Dictionary<string, EthnicityResponse>>(await FileHelper.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "ethnicities.json"));
            }
        }
    }
}