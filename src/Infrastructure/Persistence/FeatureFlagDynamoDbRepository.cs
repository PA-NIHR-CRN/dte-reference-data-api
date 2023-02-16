using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Application.Contracts;
using Application.Settings;
using Domain.Entities.FeatureFlags;
using Dte.Common.Persistence;

namespace Infrastructure.Persistence
{
    public class FeatureFlagDynamoDbRepository : BaseDynamoDbRepository, IFeatureFlagRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;
        
        private static string ServiceNameKey(string serviceName) => $"SERVICENAME#{serviceName}";
        private static string FeatureNameKey(string featureName) => $"FEATURENAME#{featureName}";

        public FeatureFlagDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context, AwsSettings awsSettings) : base(client, context)
        {
            _client = client;
            _context = context;
            _config = new DynamoDBOperationConfig { OverrideTableName = awsSettings.FeatureFlagDynamoDbTableName };
        }

        public async Task<FeatureFlag> GetFeatureFlagAsync(string serviceName, string featureName)
        {
            return await _context.LoadAsync<FeatureFlag>(ServiceNameKey(serviceName), FeatureNameKey(featureName), _config);
        }
    }
}