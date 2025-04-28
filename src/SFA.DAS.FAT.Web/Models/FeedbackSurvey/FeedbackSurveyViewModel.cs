using System;
using System.Collections.Generic;
using System.Linq;
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

    public static DateTime CurrentDate { get; set; }
    public List<FeedbackByYear> FeedbackByYear { get; set; }


    public static FeedbackSurveyViewModel ProcessFeedbackDetails(List<EmployerFeedbackAnnualSummaries> employerSummaries, List<ApprenticeFeedbackAnnualSummaries> apprenticeSummaries, DateTime currentDate)
    {
        CurrentDate = currentDate;
        var feedback = CreateFeedbackWithYearsAndHeadings();

        InjectEmployerFeedbackDetails(employerSummaries, feedback);
        InjectApprenticeFeedbackDetails(apprenticeSummaries, feedback);

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

    private static List<FeedbackByYear> CreateFeedbackWithYearsAndHeadings()
    {
        var feedbackItems = BuildFeedbackByYearList();

        var mostRecentDate =
            feedbackItems.MaxBy(x => x.StartYear)!.StartYear;

        var leastRecentDate =
            feedbackItems.Where(x => x.StartYear != 0).MinBy(x => x.StartYear)!.StartYear;

        PopulateHeadingsAndMainText(feedbackItems, leastRecentDate, mostRecentDate);

        return feedbackItems;
    }

    private static void PopulateHeadingsAndMainText(List<FeedbackByYear> feedbackItems, int leastRecentDate, int mostRecentDate)
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
                if (feedbackByYear.IsMostRecentYear)
                {
                    feedbackByYear.Heading = $"{feedbackByYear.StartYear} to today";
                    feedbackByYear.SubHeading = $"1 August {feedbackByYear.StartYear} to today";
                    feedbackByYear.MainText = EmployerMostRecentReviewsText;
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

    private static List<FeedbackByYear> BuildFeedbackByYearList()
    {
        var feedback = new List<FeedbackByYear>();

        int academicYearEnd = CurrentDate.Month > 7
            ? DateTime.UtcNow.Year + 1
            : DateTime.UtcNow.Year;

        for (int yearEnd = academicYearEnd; yearEnd > academicYearEnd - 5; yearEnd--)
        {
            feedback.Add(new FeedbackByYear
            {
                StartYear = yearEnd - 1,
                EndYear = yearEnd,
                TimePeriod = $"AY{yearEnd - 2001}{yearEnd - 2000}",
                IsMostRecentYear = yearEnd == academicYearEnd
            });
        }

        feedback.Add(new FeedbackByYear { StartYear = 0, EndYear = 0, TimePeriod = "All" });

        return feedback;
    }


}
