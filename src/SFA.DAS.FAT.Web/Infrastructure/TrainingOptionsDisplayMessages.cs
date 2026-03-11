using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.FAT.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class TrainingOptionsDisplayMessages
    {
        public const string OnlineTraining = "Your learner can complete this training remotely.";

        public const string AtApprenticeWorkplaceWithNoLocationNational = "The training provider can travel to you to deliver this course.";
        public const string AtApprenticeWorkplaceWithNoLocationRegional = "Training is provided at learner's workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the learner's workplace in your location.";
        public const string AtApprenticeWorkplaceWithLocationMatchingRegional = "The training provider can travel to you to deliver this course.";
        public const string AtApprenticeWorkplaceWithLocationNotMatchingRegional = "This provider has said they do not deliver training at learner's workplaces in your region.The training provider can travel to you to deliver this course.";

        public const string AtProviderPlaceWithNoLocation = "Training is provided at learner’s workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the learner’s workplace in your location.";
        public const string ProviderClosestLocationWithDistance = "Training provider’s closest location is <strong>{0} miles away:</strong> {1}";

        public const string BlockReleaseHint = "Training in blocks of a week or more at the provider's location.";
        public const string BlockReleaseMultipleLocations = "Block release at multiple locations.";
        public const string ViewAllBlockReleaseLocations = "View all block release locations";

        public const string ClosestLocationWithDistance = "<strong>{0} miles away:</strong> to closest location: {1}";

        public const string DayReleaseHint = "One day a week at the provider's location.";
        public const string DayReleaseMultipleLocations = "Day release at multiple locations.";
        public const string ViewAllDayReleaseLocations = "View all day release locations";
    }
}
