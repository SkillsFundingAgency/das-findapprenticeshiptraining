using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;
using SFA.DAS.FAT.Web.Models.Providers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ProviderDetailsViewModelTests;
public class WhenBuildingProviderDetailsViewModelApprenticeFeedbackTests
{

    public const string TimePeriod1 = "AY2425";
    public const string TimePeriod2 = "AY2324";
    public const string TimePeriodAll = "All";
    public DateTime DateToCheck = new(2025, 4, 28);

    [Test, MoqInlineAutoData]
    public void Then_Apprentice_Feedback_Details_As_Expected_First_Tag(GetProviderQueryResponse response, string feedbackName, int agree, int disagree, int reviewCount, int stars)
    {
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = GetApprenticeAnnualSummaries(feedbackName, agree, disagree, reviewCount, stars);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, DateToCheck);

        var totalCount = agree + disagree;
        int expectedAgreePerc = (int)((totalCount == 0) ? 0 : Math.Round(((double)agree * 100) / totalCount));
        int expectedDisagreePerc = 100 - expectedAgreePerc;
        var firstItem = sut.FeedbackSurvey.FeedbackByYear[0];
        var firstFeedbackDetail = firstItem.ApprenticeFeedbackDetails.ProviderAttributes[0];
        using (new AssertionScope())
        {
            sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);

            firstItem.IsMostRecentYear.Should().Be(true);
            firstItem.EndYear.Should().Be(2025);
            firstItem.StartYear.Should().Be(2024);
            firstItem.Heading.Should().Be("2024 to today");
            firstItem.SubHeading.Should().Be("1 August 2024 to today");
            firstItem.MainText.Should()
                .Be(FeedbackSurveyViewModel.EmployerMostRecentReviewsText);
            firstItem.NoApprenticeReviewsText.Should()
                .Be(FeedbackSurveyViewModel.ApprenticeNoResultsRecentTab);
            firstItem.ShowEmployerFeedbackStars.Should().Be(false);
            firstItem.ShowApprenticeFeedbackStars.Should().Be(true);
            firstItem.TimePeriod.Should().Be(TimePeriod1);

            firstFeedbackDetail.TotalCount.Should().Be(agree + disagree);
            firstFeedbackDetail.Agree.Should().Be(agree);
            firstFeedbackDetail.Disagree.Should().Be(disagree);
            firstFeedbackDetail.AgreePerc.Should().Be(expectedAgreePerc);
            firstFeedbackDetail.DisagreePerc.Should().Be(expectedDisagreePerc);
        }
    }

    [Test, MoqInlineAutoData]
    public void Then_Apprentice_Feedback_Details_When_No_Data(GetProviderQueryResponse response)
    {
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = GetApprenticeAnnualSummaries("test", 0, 0, 0, 1);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, DateToCheck);

        int expectedAgreePerc = 0;
        int expectedDisagreePerc = 0;
        var firstItem = sut.FeedbackSurvey.FeedbackByYear[0];
        var firstFeedbackDetail = firstItem.ApprenticeFeedbackDetails.ProviderAttributes[0];
        using (new AssertionScope())
        {
            sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);

            firstItem.IsMostRecentYear.Should().Be(true);
            firstItem.EndYear.Should().Be(2025);
            firstItem.StartYear.Should().Be(2024);
            firstItem.Heading.Should().Be("2024 to today");
            firstItem.SubHeading.Should().Be("1 August 2024 to today");
            firstItem.MainText.Should()
                .Be(FeedbackSurveyViewModel.EmployerMostRecentReviewsText);
            firstItem.NoApprenticeReviewsText.Should()
                .Be(FeedbackSurveyViewModel.ApprenticeNoResultsRecentTab);
            firstItem.ShowEmployerFeedbackStars.Should().Be(false);
            firstItem.ShowApprenticeFeedbackStars.Should().Be(true);
            firstItem.TimePeriod.Should().Be(TimePeriod1);

            firstFeedbackDetail.TotalCount.Should().Be(0);
            firstFeedbackDetail.Agree.Should().Be(0);
            firstFeedbackDetail.Disagree.Should().Be(0);
            firstFeedbackDetail.AgreePerc.Should().Be(expectedAgreePerc);
            firstFeedbackDetail.DisagreePerc.Should().Be(expectedDisagreePerc);
        }
    }

    [Test, MoqInlineAutoData]
    public void Then_Apprentice_Feedback_Details_As_Expected_Second_Tag(GetProviderQueryResponse response, string feedbackName, int agree, int disagree, int reviewCount, int stars)
    {
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = GetApprenticeAnnualSummaries(feedbackName, agree, disagree, reviewCount, stars);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, DateToCheck);

        var totalCount = agree + disagree;
        int expectedAgreePerc = (int)((totalCount == 0) ? 0 : Math.Round(((double)(agree + 1) * 100) / totalCount));
        int expectedDisagreePerc = 100 - expectedAgreePerc;

        sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);
        var feedbackTab = sut.FeedbackSurvey.FeedbackByYear[1];
        feedbackTab.IsMostRecentYear.Should().Be(false);
        feedbackTab.EndYear.Should().Be(2024);
        feedbackTab.StartYear.Should().Be(2023);
        feedbackTab.Heading.Should().Be("2023 to 2024");
        feedbackTab.SubHeading.Should().Be("1 August 2023 to 31 July 2024");
        feedbackTab.MainText.Should().Be(FeedbackSurveyViewModel.AllCoursesDeliveredTextLastFullYear);
        feedbackTab.NoApprenticeReviewsText.Should().Be(FeedbackSurveyViewModel.ApprenticeNoResultsPastTab);
        feedbackTab.ShowEmployerFeedbackStars.Should().Be(false);
        feedbackTab.ShowApprenticeFeedbackStars.Should().Be(true);
        feedbackTab.TimePeriod.Should().Be(TimePeriod2);
        var feedbackDetail = feedbackTab.ApprenticeFeedbackDetails.ProviderAttributes[0];
        feedbackDetail.TotalCount.Should().Be(agree + disagree);
        feedbackDetail.Agree.Should().Be(agree + 1);
        feedbackDetail.Disagree.Should().Be(disagree - 1);
        feedbackDetail.AgreePerc.Should().Be(expectedAgreePerc);
        feedbackDetail.DisagreePerc.Should().Be(expectedDisagreePerc);
    }

    [Test, MoqInlineAutoData]
    public void Then_Apprentice_Feedback_Details_As_Expected_Overall_Tag(GetProviderQueryResponse response, string feedbackName, int agree, int disagree, int reviewCount, int stars)
    {
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = GetApprenticeAnnualSummaries(feedbackName, agree, disagree, reviewCount, stars);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, DateToCheck);

        var totalCount = agree + disagree;
        int expectedAgreePerc = (int)((totalCount == 0) ? 0 : Math.Round(((double)(agree + 2) * 100) / totalCount));
        int expectedDisagreePerc = 100 - expectedAgreePerc;

        sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);
        var feedbackTab = sut.FeedbackSurvey.FeedbackByYear[5];
        feedbackTab.IsMostRecentYear.Should().Be(false);
        feedbackTab.EndYear.Should().Be(0);
        feedbackTab.StartYear.Should().Be(0);
        feedbackTab.Heading.Should().Be("Overall reviews");
        feedbackTab.SubHeading.Should().Be("1 August 2020 to today");
        feedbackTab.MainText.Should().Be(FeedbackSurveyViewModel.EmployerReviewsOverallText);
        feedbackTab.NoApprenticeReviewsText.Should().Be(FeedbackSurveyViewModel.ApprenticeNoResultsPastTab);
        feedbackTab.ShowEmployerFeedbackStars.Should().Be(false);
        feedbackTab.ShowApprenticeFeedbackStars.Should().Be(true);
        feedbackTab.TimePeriod.Should().Be(TimePeriodAll);
        var feedbackDetail = feedbackTab.ApprenticeFeedbackDetails.ProviderAttributes[0];
        feedbackDetail.TotalCount.Should().Be(agree + disagree);
        feedbackDetail.Agree.Should().Be(agree + 2);
        feedbackDetail.Disagree.Should().Be(disagree - 2);
        feedbackDetail.AgreePerc.Should().Be(expectedAgreePerc);
        feedbackDetail.DisagreePerc.Should().Be(expectedDisagreePerc);
    }

    private static List<ApprenticeFeedbackAnnualSummaries> GetApprenticeAnnualSummaries(string feedbackName, int agree, int disagree, int reviewCount, int stars)
    {
        var annualSummaryItem = new AttributeResultModel { Name = feedbackName, Agree = agree, Disagree = disagree };
        var annualSummaryItem2 = new AttributeResultModel { Name = feedbackName + "2", Agree = agree + 1, Disagree = disagree - 1 };
        var annualSummaryItem3 = new AttributeResultModel { Name = feedbackName + "3", Agree = agree + 2, Disagree = disagree - 2 };

        var summaries = new List<ApprenticeFeedbackAnnualSummaries>
        {
            new()
            {
                ReviewCount = reviewCount,
                Stars = stars,
                TimePeriod = TimePeriod1,
                ProviderAttribute = new() { annualSummaryItem}
            },
            new()
            {
                ReviewCount = reviewCount+1,
                Stars = stars+1,
                TimePeriod = TimePeriod2,
                ProviderAttribute = new() { annualSummaryItem2}
            },
            new()
            {
                ReviewCount = reviewCount+2,
                Stars = stars+2,
                TimePeriod = TimePeriodAll,
                ProviderAttribute = new() { annualSummaryItem3}
            },

        };

        return summaries;
    }
}
