using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.FAT.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RouteNames
    {
        public const string ServiceStartDefault = "default";
        public const string ServiceStart = "service-start";
        public const string Cookies = "cookies";
        public const string CookiesDetails = "cookies-details";

        public const string Courses = "courses";
        public const string CourseDetails = "course-details";
        public const string CourseProviders = "course-providers";
        public const string CourseProviderDetails = "course-provider-details";

        public const string Provider = nameof(Provider);

        public const string ShortLists = "shortlist";
        public const string CreateShortlistItem = "create-shortlist";
        public const string DeleteShortlistItem = "delete-shortlist";
        public const string OpenShortlistItem = "open-shortlist-item";

        public const string AccessibilityStatement = "accessibility-statement";

        public const string Privacy = "privacy";

        public const string GetRegisteredProviders = nameof(GetRegisteredProviders);
        public const string SelectProvider = nameof(SelectProvider);
        public const string SearchCourses = nameof(SearchCourses);
    }
}
