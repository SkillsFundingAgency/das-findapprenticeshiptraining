using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.FeedbackSurvey;

public class FeedbackSurveyViewModel
{
    public const string AllCoursesDeliveredText = "These reviews are for all courses delivered by this provider.";
    public const string EmployerReviewsOverallText = "These reviews are for all courses delivered by this provider in the last 5 academic years.";
    public const string EmployerMostRecentReviewsText = "These are the most recent reviews for this training provider.  We update the main star rating on this page when we have reviews from a full academic year.";
    public const string EmployersNoResultsPastTab = "No results. Not enough employers gave feedback about this training provider.";
    public const string EmployersNoResultsRecentTab = "No results. Not enough employers have given feedback about this training provider.";

    public const string ApprenticeNoResultsRecentTab = "No results. Not enough apprentices have given feedback about this training provider.";
    public const string ApprenticeNoResultsPastTab = "No results. Not enough apprentices gave feedback about this training provider.";

    public bool ShowSurvey => FeedbackByYear is { Count: > 0 };

    public List<FeedbackByYear> FeedbackByYear { get; set; }


    public static FeedbackSurveyViewModel ProcessFeedbackDetails(GetProviderQueryResponse source)
    {
        var feedback = CreateFeedbackWithYearsAndHeadings(source);

        InjectEmployerFeedbackDetails(source.AnnualEmployerFeedbackDetails, feedback);
        InjectApprenticeFeedbackDetails(source.AnnualApprenticeFeedbackDetails, feedback);

        return new FeedbackSurveyViewModel { FeedbackByYear = feedback };
    }

    public static FeedbackSurveyViewModel ProcessFeedbackDetails(GetCourseProviderQueryResult source)
    {
        var feedback = CreateFeedbackWithYearsAndHeadings(source);
        InjectEmployerFeedbackDetails(source.AnnualEmployerFeedbackDetails.ToList(), feedback);
        InjectApprenticeFeedbackDetails(source.AnnualApprenticeFeedbackDetails.ToList(), feedback);


        return new FeedbackSurveyViewModel { FeedbackByYear = feedback };
    }


    private static void InjectApprenticeFeedbackDetails(List<ApprenticeFeedbackAnnualSummaries> apprenticeFeedback, List<FeedbackByYear> feedback)
    {
        if (apprenticeFeedback == null || feedback.Count == 0) return;

        foreach (var item in feedback)
        {
            item.NoApprenticeReviewsText = item.IsMostRecentYear
                ? ApprenticeNoResultsRecentTab
                : ApprenticeNoResultsPastTab;

            foreach (var matchingFeedback in apprenticeFeedback.Where(x => x.TimePeriod == item.TimePeriod))
            {
                item.ApprenticeFeedbackDetails = matchingFeedback;
            }
        }
    }

    private static void InjectApprenticeFeedbackDetails(List<AnnualApprenticeFeedbackDetailsModel> apprenticeFeedback, List<FeedbackByYear> feedback)
    {
        if (apprenticeFeedback == null || feedback.Count == 0) return;

        foreach (var item in feedback)
        {
            item.NoApprenticeReviewsText = item.IsMostRecentYear
                ? ApprenticeNoResultsRecentTab
                : ApprenticeNoResultsPastTab;

            foreach (var matchingFeedback in apprenticeFeedback.Where(x => x.TimePeriod == item.TimePeriod))
            {
                item.ApprenticeFeedbackDetails = matchingFeedback;
            }
        }
    }


    private static void InjectEmployerFeedbackDetails(List<EmployerFeedbackAnnualSummaries> employerFeedback, List<FeedbackByYear> feedback)
    {
        if (employerFeedback == null || feedback.Count == 0) return;
        foreach (var item in feedback)
        {
            item.NoEmployerReviewsText = item.IsMostRecentYear
                ? EmployersNoResultsRecentTab
                : EmployersNoResultsPastTab;

            foreach (var matchingFeedback in employerFeedback.Where(x => x.TimePeriod == item.TimePeriod))
            {
                item.EmployerFeedbackDetails = matchingFeedback;
            }
        }
    }

    private static void InjectEmployerFeedbackDetails(List<AnnualEmployerFeedbackDetailsModel> employerFeedback, List<FeedbackByYear> feedback)
    {
        if (employerFeedback == null || feedback.Count == 0) return;
        foreach (var item in feedback)
        {
            item.NoEmployerReviewsText = item.IsMostRecentYear
                ? EmployersNoResultsRecentTab
                : EmployersNoResultsPastTab;

            foreach (var matchingFeedback in employerFeedback.Where(x => x.TimePeriod == item.TimePeriod))
            {
                item.EmployerFeedbackDetails = matchingFeedback;
            }
        }
    }

    private static List<FeedbackByYear> CreateFeedbackWithYearsAndHeadings(GetProviderQueryResponse source)
    {
        var employerTimePeriods = new List<string>();

        if (source.AnnualEmployerFeedbackDetails is { Count: > 0 })
        {
            employerTimePeriods = source.AnnualEmployerFeedbackDetails.Select(x => x.TimePeriod).ToList();
        }

        var apprenticeTimePeriods = new List<string>();

        if (source.AnnualApprenticeFeedbackDetails is { Count: > 0 })
        {
            apprenticeTimePeriods = source.AnnualApprenticeFeedbackDetails.Select(x => x.TimePeriod).ToList();
        }

        var allTimePeriods = employerTimePeriods.ToList();
        var feedbackItems = BuildFeedbackByYearList(apprenticeTimePeriods, allTimePeriods);

        if (feedbackItems.Count > 0)
        {
            var mostRecentDate =
                feedbackItems.MaxBy(x => x.StartYear)!.StartYear;

            var leastRecentDate =
                feedbackItems.OrderBy(x => x.StartYear).First(x => x.StartYear != 0)!.StartYear;

            PopulateFeedbackItems(feedbackItems, leastRecentDate, mostRecentDate);
        }

        return feedbackItems.OrderByDescending(f => f.StartYear).ToList();
    }

    private static List<FeedbackByYear> CreateFeedbackWithYearsAndHeadings(GetCourseProviderQueryResult source)
    {
        var employerTimePeriods = new List<string>();

        if (source.AnnualEmployerFeedbackDetails.ToList() is { Count: > 0 })
        {
            employerTimePeriods = source.AnnualEmployerFeedbackDetails.Select(x => x.TimePeriod).ToList();
        }

        var apprenticeTimePeriods = new List<string>();

        if (source.AnnualApprenticeFeedbackDetails.ToList() is { Count: > 0 })
        {
            apprenticeTimePeriods = source.AnnualApprenticeFeedbackDetails.Select(x => x.TimePeriod).ToList();
        }

        var allTimePeriods = employerTimePeriods.ToList();
        var feedbackItems = BuildFeedbackByYearList(apprenticeTimePeriods, allTimePeriods);

        if (feedbackItems.Count > 0)
        {
            var mostRecentDate =
                feedbackItems.MaxBy(x => x.StartYear)!.StartYear;

            var leastRecentDate =
                feedbackItems.OrderBy(x => x.StartYear).First(x => x.StartYear != 0)!.StartYear;

            PopulateFeedbackItems(feedbackItems, leastRecentDate, mostRecentDate);
        }

        return feedbackItems.OrderByDescending(f => f.StartYear).ToList();
    }

    private static void PopulateFeedbackItems(List<FeedbackByYear> feedbackItems, int leastRecentDate, int mostRecentDate)
    {
        foreach (var feedbackByYear in feedbackItems)
        {
            if (feedbackByYear.StartYear == 0)
            {
                feedbackByYear.Heading = "Overall reviews";
                feedbackByYear.SubHeading = $"1 August {leastRecentDate} to 31 July {mostRecentDate}";
                feedbackByYear.MainText = EmployerReviewsOverallText;
            }
            else
            {
                if (feedbackByYear.StartYear == mostRecentDate)
                {
                    feedbackByYear.Heading = $"{feedbackByYear.StartYear} to today";
                    feedbackByYear.SubHeading = $"1 August {feedbackByYear.StartYear} to today";
                    feedbackByYear.MainText = EmployerMostRecentReviewsText;
                    feedbackByYear.IsMostRecentYear = true;
                }
                else
                {
                    feedbackByYear.Heading = $"{feedbackByYear.StartYear} to {feedbackByYear.EndYear}";
                    feedbackByYear.SubHeading = $"1 August {feedbackByYear.StartYear} to 31 July {feedbackByYear.EndYear}";
                    feedbackByYear.MainText = AllCoursesDeliveredText;
                }
            }
        }
    }

    private static List<FeedbackByYear> BuildFeedbackByYearList(List<string> apprenticeTimePeriods, List<string> allTimePeriods)
    {
        var feedback = new List<FeedbackByYear>();

        foreach (var timePeriods in apprenticeTimePeriods)
        {
            if (!allTimePeriods.Contains(timePeriods)) allTimePeriods.Add(timePeriods);
        }

        foreach (var timePeriod in allTimePeriods)
        {
            var startYear = 0;
            var endYear = 0;

            if (!timePeriod.Contains("All") && timePeriod.StartsWith("AY") && timePeriod.Length == 6)
            {
                startYear = int.TryParse(timePeriod.AsSpan(2, 2), out var starterYear) ? starterYear + 2000 : 0;
                endYear = int.TryParse(timePeriod.AsSpan(4, 2), out var enderYear) ? enderYear + 2000 : 0;
            }

            feedback.Add(new FeedbackByYear
            {
                StartYear = startYear,
                EndYear = endYear,
                TimePeriod = timePeriod
            });
        }

        return feedback;
    }


}
