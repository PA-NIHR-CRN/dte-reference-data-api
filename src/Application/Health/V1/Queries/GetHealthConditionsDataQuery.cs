using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dte.Common.Helpers;
using MediatR;

namespace Application.Health.V1.Queries
{
    public class GetHealthConditionsDataQuery : IRequest<string[]>
    {
        public GetHealthConditionsDataQuery()
        {
        }

        public class GetHealthConditionsDataQueryHandler : IRequestHandler<GetHealthConditionsDataQuery, string[]>
        {
            public GetHealthConditionsDataQueryHandler()
            {
            }

            public async Task<string[]> Handle(GetHealthConditionsDataQuery request, CancellationToken cancellationToken)
            {
                var nhsList = await GetHealthConditionsArrayFromResource("NHSHealthConditions.txt");           
                var bporList = await GetHealthConditionsArrayFromResource("BPORHealthConditions.txt");

                return nhsList.Union(bporList).ToArray();
            }

            private async Task<string[]> GetHealthConditionsArrayFromResource(string resourceName)
            {
                var resourceList = await FileHelper.GetEmbeddedResource(Assembly.GetExecutingAssembly(), resourceName);
                var resourceLines = resourceList.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var resourceMultiLine = resourceLines.Select(x => x.Split(";"));
                return resourceMultiLine.SelectMany(x => x.SelectMany(y => y.Split("|"))).ToArray();
            }
        }
    }
}