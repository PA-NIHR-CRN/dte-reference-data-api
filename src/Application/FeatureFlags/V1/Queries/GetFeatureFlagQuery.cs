using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.FeatureFlags.V1.Queries
{
    public class GetFeatureFlagQuery : IRequest<FeatureFlagResponse>
    {
        public string ServiceName { get; }
        public string FeatureName { get; }

        public GetFeatureFlagQuery(string serviceName, string featureName)
        {
            ServiceName = serviceName;
            FeatureName = featureName;
        }

        public class GetFeatureFlagQueryHandler : IRequestHandler<GetFeatureFlagQuery, FeatureFlagResponse>
        {
            private readonly IFeatureFlagRepository _featureFlagRepository;
            private readonly ILogger<GetFeatureFlagQueryHandler> _logger;

            public GetFeatureFlagQueryHandler(IFeatureFlagRepository featureFlagRepository, ILogger<GetFeatureFlagQueryHandler> logger)
            {
                _featureFlagRepository = featureFlagRepository;
                _logger = logger;
            }

            public async Task<FeatureFlagResponse> Handle(GetFeatureFlagQuery request, CancellationToken cancellationToken)
            {
                var featureFlag = await _featureFlagRepository.GetFeatureFlagAsync(request.ServiceName, request.FeatureName);

                if (featureFlag == null)
                {
                    _logger.LogWarning($"Feature flag for service: {request.ServiceName} and for feature: {request.FeatureName} not found");
                    return new FeatureFlagResponse { Enabled = false, Found = false};
                }

                return new FeatureFlagResponse { Enabled = featureFlag.Enabled, Found = true};
            }
        }
    }
}