namespace Application.Settings
{
    public class AwsSettings
    {
        public static string SectionName => "AwsSettings";
        public string FeatureFlagDynamoDbTableName { get; set; }
        public string ServiceUrl { get; set; }
    }
}