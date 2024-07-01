namespace SFA.DAS.FAT.Domain.Configuration
{
    public class FindApprenticeshipTrainingWeb
    {
        public string RedisConnectionString { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
        public string ZendeskSectionId { get; set; }
        public string ZendeskSnippetKey { get; set; }
        public string ZendeskCoBrowsingSnippetKey { get; set; }
        public bool EmployerDemandFeatureToggle { get; set; }
        public string EmployerDemandUrl { get; set; }
        public string EmployerAccountsUrl { get; set; }
        public string RequestApprenticeshipTrainingUrl { get; set; }
    }
}
