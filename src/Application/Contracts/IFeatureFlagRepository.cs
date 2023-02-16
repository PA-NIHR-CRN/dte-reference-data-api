using System.Threading.Tasks;
using Domain.Entities.FeatureFlags;

namespace Application.Contracts
{
    public interface IFeatureFlagRepository
    {
        Task<FeatureFlag> GetFeatureFlagAsync(string serviceName, string featureName);
    }
}