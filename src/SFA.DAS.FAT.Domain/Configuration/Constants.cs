namespace SFA.DAS.FAT.Domain.Configuration
{
    public class Constants
    {
        public static string LocationCookieName => "SFA.DAS.FindApprenticeshipTraining.SearchLocation";
        public static string ProvidersCookieName => "SFA.DAS.FindApprenticeshipTraining.SearchProviders";
        public static string ShortlistCookieName => "SFA.DAS.FindApprenticeshipTraining.Shortlist";
        public static string GaDataProtectorName => "Analytics";
        public static string ShortlistProtectorName => "Shortlist";

        public static int DefaultDistance => 10;

        public static int DefaultPageSize => 10;
    }
}
