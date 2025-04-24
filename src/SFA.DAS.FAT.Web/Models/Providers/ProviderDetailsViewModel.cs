using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderDetailsViewModel : PageLinksViewModelBase
{


    public int Ukprn { get; set; }
    public string ProviderName { get; set; }

    public string ProviderAddress { get; set; }
    public ContactDetailsModel Contact { get; set; }
    public ProviderCoursesModel ProviderCoursesDetails { get; set; }

    public ProviderQarModel Qar { get; set; }

    public EndpointAssessmentModel EndpointAssessments { get; set; }

    public ReviewsModel Reviews { get; set; }

    public bool ShowCourses => ProviderCoursesDetails.CourseCount > 0;

    public bool EmployerReviewed => Reviews.EmployerRating != ProviderRating.NotYetReviewed.ToString();
    public bool ApprenticeReviewed => Reviews.ApprenticeRating != ProviderRating.NotYetReviewed.ToString();


    public FeedbackSurveyViewModel FeedbackSurvey { get; set; }

    public static implicit operator ProviderDetailsViewModel(GetProviderQueryResponse source)
    {
        var vm = new ProviderDetailsViewModel
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            ProviderAddress = ((GetProviderAddress)source.ProviderAddress).GetComposedAddress(source.ProviderName),
            Contact = source.Contact,
            ProviderCoursesDetails = source.Courses,
            Qar = source.Qar,
            Reviews = source.Reviews,
            EndpointAssessments = source.EndpointAssessments,
            FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(source),
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };

        if (vm.ProviderCoursesDetails is { Courses: { } })
        {
            foreach (var course in vm.ProviderCoursesDetails.Courses)
            {
                var routeData = new Dictionary<string, string>
                {
                    { "id", course.LarsCode.ToString() },
                    { "providerId", vm.Ukprn.ToString() }
                };

                course.RouteData = routeData;
            }
        }

        return vm;
    }

    // private static void InjectApprenticeFeedbackDetails(List<ApprenticeFeedbackAnnualSummaries> apprenticeFeedback, List<FeedbackByYear> feedback)
    // {
    //     if (apprenticeFeedback == null || feedback.Count == 0) return;
    //
    //     foreach (var item in feedback)
    //     {
    //         item.NoApprenticeReviewsText = item.IsMostRecentYear
    //             ? ApprenticeNoResultsRecentTab
    //             : ApprenticeNoResultsPastTab;
    //
    //         foreach (var matchingFeedback in apprenticeFeedback.Where(x => x.TimePeriod == item.TimePeriod))
    //         {
    //             item.ApprenticeFeedbackDetails = matchingFeedback;
    //         }
    //     }
    // }
    // private static void InjectEmployerFeedbackDetails(List<EmployerFeedbackAnnualSummaries> employerFeedback, List<FeedbackByYear> feedback)
    // {
    //     if (employerFeedback == null || feedback.Count == 0) return;
    //     foreach (var item in feedback)
    //     {
    //         item.NoEmployerReviewsText = item.IsMostRecentYear
    //             ? EmployersNoResultsRecentTab
    //             : EmployersNoResultsPastTab;
    //
    //         foreach (var matchingFeedback in employerFeedback.Where(x => x.TimePeriod == item.TimePeriod))
    //         {
    //             item.EmployerFeedbackDetails = matchingFeedback;
    //         }
    //     }
    // }

    // private static List<FeedbackByYear> ProcessFeedbackDetails(GetProviderQueryResponse source)
    // {
    //     var feedback = CreateFeedbackWithYearsAndHeadings(source);
    //
    //     InjectEmployerFeedbackDetails(source.AnnualEmployerFeedbackDetails, feedback);
    //     InjectApprenticeFeedbackDetails(source.AnnualApprenticeFeedbackDetails, feedback);
    //
    //     return feedback;
    // }
    //
    // private static List<FeedbackByYear> CreateFeedbackWithYearsAndHeadings(GetProviderQueryResponse source)
    // {
    //     var employerTimePeriods = new List<string>();
    //
    //     if (source.AnnualEmployerFeedbackDetails is { Count: > 0 })
    //     {
    //         employerTimePeriods = source.AnnualEmployerFeedbackDetails.Select(x => x.TimePeriod).ToList();
    //     }
    //
    //     var apprenticeTimePeriods = new List<string>();
    //
    //     if (source.AnnualApprenticeFeedbackDetails is { Count: > 0 })
    //     {
    //         apprenticeTimePeriods = source.AnnualApprenticeFeedbackDetails.Select(x => x.TimePeriod).ToList();
    //     }
    //
    //     var allTimePeriods = employerTimePeriods.ToList();
    //     List<FeedbackByYear> feedbackItems = BuildFeedbackByYearList(apprenticeTimePeriods, allTimePeriods);
    //
    //     if (feedbackItems.Count > 0)
    //     {
    //         var mostRecentDate =
    //             feedbackItems.MaxBy(x => x.StartYear)!.StartYear;
    //
    //         var leastRecentDate =
    //             feedbackItems.OrderBy(x => x.StartYear).First(x => x.StartYear != 0)!.StartYear;
    //
    //         PopulateFeedbackItems(feedbackItems, leastRecentDate, mostRecentDate);
    //     }
    //
    //     return feedbackItems.OrderByDescending(f => f.StartYear).ToList();
    // }
    //
    // private static void PopulateFeedbackItems(List<FeedbackByYear> feedbackItems, int leastRecentDate, int mostRecentDate)
    // {
    //     foreach (var feedbackByYear in feedbackItems)
    //     {
    //         if (feedbackByYear.StartYear == 0)
    //         {
    //             feedbackByYear.Heading = "Overall reviews";
    //             feedbackByYear.SubHeading = $"1 August {leastRecentDate} to 31 July {mostRecentDate}";
    //             feedbackByYear.MainText = EmployerReviewsOverallText;
    //         }
    //         else
    //         {
    //             if (feedbackByYear.StartYear == mostRecentDate)
    //             {
    //                 feedbackByYear.Heading = $"{feedbackByYear.StartYear} to today";
    //                 feedbackByYear.SubHeading = $"1 August {feedbackByYear.StartYear} to today";
    //                 feedbackByYear.MainText = EmployerMostRecentReviewsText;
    //                 feedbackByYear.IsMostRecentYear = true;
    //             }
    //             else
    //             {
    //                 feedbackByYear.Heading = $"{feedbackByYear.StartYear} to {feedbackByYear.EndYear}";
    //                 feedbackByYear.SubHeading = $"1 August {feedbackByYear.StartYear} to 31 July {feedbackByYear.EndYear}";
    //                 feedbackByYear.MainText = AllCoursesDeliveredText;
    //             }
    //         }
    //     }
    // }
    //
    // private static List<FeedbackByYear> BuildFeedbackByYearList(List<string> apprenticeTimePeriods, List<string> allTimePeriods)
    // {
    //     List<FeedbackByYear> feedback = new List<FeedbackByYear>();
    //
    //     foreach (var timePeriods in apprenticeTimePeriods)
    //     {
    //         if (!allTimePeriods.Contains(timePeriods)) allTimePeriods.Add(timePeriods);
    //     }
    //
    //     foreach (var timePeriod in allTimePeriods)
    //     {
    //         var startYear = 0;
    //         var endYear = 0;
    //
    //         if (!timePeriod.Contains("All") && timePeriod.StartsWith("AY") && timePeriod.Length == 6)
    //         {
    //             startYear = int.TryParse(timePeriod.AsSpan(2, 2), out var starterYear) ? starterYear + 2000 : 0;
    //             endYear = int.TryParse(timePeriod.AsSpan(4, 2), out var enderYear) ? enderYear + 2000 : 0;
    //         }
    //
    //         feedback.Add(new FeedbackByYear
    //         {
    //             StartYear = startYear,
    //             EndYear = endYear,
    //             TimePeriod = timePeriod
    //         });
    //     }
    //
    //     return feedback;
    // }
}
