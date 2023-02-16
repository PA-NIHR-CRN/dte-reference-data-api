using Amazon.DynamoDBv2.DataModel;

namespace Domain.Entities.FeatureFlags
{
    public class FeatureFlag
    {
        [DynamoDBHashKey("PK")] public string Pk { get; set; }
        [DynamoDBRangeKey("SK")] public string Sk { get; set; }

        [DynamoDBProperty] public string ServiceName { get; set; }
        [DynamoDBProperty] public string FeatureName { get; set; }
        [DynamoDBProperty] public string Platform { get; set; }
        [DynamoDBProperty] public string Os { get; set; }
        [DynamoDBProperty] public string JiraNumber { get; set; }
        [DynamoDBProperty] public bool Enabled { get; set; }
    }
}