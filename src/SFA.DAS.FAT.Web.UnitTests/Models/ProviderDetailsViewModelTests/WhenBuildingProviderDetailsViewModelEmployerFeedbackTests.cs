using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;
using SFA.DAS.FAT.Web.Models.Providers;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ProviderDetailsViewModelTests;
public class WhenBuildingProviderDetailsViewModelEmployerFeedbackTests
{

    public const string TimePeriod1 = "AY2425";
    public const string TimePeriod2 = "AY2324";
    public const string TimePeriodAll = "All";


    [Test, MoqInlineAutoData]
    public void Then_Employer_Feedback_Details_As_Expected_First_Tag(GetProviderQueryResponse response, string feedbackName, int strength, int weakness, int reviewCount, int stars)
    {
        response.AnnualApprenticeFeedbackDetails = null;
        response.AnnualEmployerFeedbackDetails = GetEmployerAnnualSummaries(feedbackName, strength, weakness, reviewCount, stars);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, new DateTimeService().GetDateTime());

        var totalCount = strength + weakness;
        int expectedStrengthPerc = (int)((totalCount == 0) ? 0 : Math.Round(((double)strength * 100) / totalCount));
        int expectedWeaknessPerc = 100 - expectedStrengthPerc;
        var firstItem = sut.FeedbackSurvey.FeedbackByYear[0];
        var firstFeedbackDetail = firstItem.EmployerFeedbackDetails.ProviderAttributes[0];
        using (new AssertionScope())
        {
            sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);

            firstItem.IsMostRecentYear.Should().Be(true);
            firstItem.EndYear.Should().Be(2025);
            firstItem.StartYear.Should().Be(2024);
            firstItem.Heading.Should().Be("2024 to today");
            firstItem.SubHeading.Should().Be("1 August 2024 to today");
            firstItem.MainText.Should()
                .Be(
                   FeedbackSurveyViewModel.EmployerMostRecentReviewsText);
            firstItem.NoEmployerReviewsText.Should()
                .Be(FeedbackSurveyViewModel.EmployersNoResultsRecentTab);
            firstItem.ShowEmployerFeedbackStars.Should().Be(true);
            firstItem.ShowApprenticeFeedbackStars.Should().Be(false);
            firstItem.TimePeriod.Should().Be(TimePeriod1);

            firstFeedbackDetail.TotalCount.Should().Be(strength + weakness);
            firstFeedbackDetail.Strength.Should().Be(strength);
            firstFeedbackDetail.Weakness.Should().Be(weakness);
            firstFeedbackDetail.StrengthPerc.Should().Be(expectedStrengthPerc);
            firstFeedbackDetail.WeaknessPerc.Should().Be(expectedWeaknessPerc);
        }
    }

    [Test, MoqInlineAutoData]
    public void Then_Employer_Feedback_Details_As_Expected_Second_Tag(GetProviderQueryResponse response, string feedbackName, int strength, int weakness, int reviewCount, int stars)
    {
        response.AnnualApprenticeFeedbackDetails = null;
        response.AnnualEmployerFeedbackDetails = GetEmployerAnnualSummaries(feedbackName, strength, weakness, reviewCount, stars);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, new DateTimeService().GetDateTime());

        var totalCount = strength + weakness;
        int expectedStrengthPerc = (int)((totalCount == 0) ? 0 : Math.Round(((double)(strength + 1) * 100) / totalCount));
        int expectedWeaknessPerc = 100 - expectedStrengthPerc;

        sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);
        var feedbackTab = sut.FeedbackSurvey.FeedbackByYear[1];
        feedbackTab.IsMostRecentYear.Should().Be(false);
        feedbackTab.EndYear.Should().Be(2024);
        feedbackTab.StartYear.Should().Be(2023);
        feedbackTab.Heading.Should().Be("2023 to 2024");
        feedbackTab.SubHeading.Should().Be("1 August 2023 to 31 July 2024");
        feedbackTab.MainText.Should().Be(FeedbackSurveyViewModel.AllCoursesDeliveredText);
        feedbackTab.NoEmployerReviewsText.Should().Be(FeedbackSurveyViewModel.EmployersNoResultsPastTab);
        feedbackTab.ShowEmployerFeedbackStars.Should().Be(true);
        feedbackTab.ShowApprenticeFeedbackStars.Should().Be(false);
        feedbackTab.TimePeriod.Should().Be(TimePeriod2);
        var feedbackDetail = feedbackTab.EmployerFeedbackDetails.ProviderAttributes[0];
        feedbackDetail.TotalCount.Should().Be(strength + weakness);
        feedbackDetail.Strength.Should().Be(strength + 1);
        feedbackDetail.Weakness.Should().Be(weakness - 1);
        feedbackDetail.StrengthPerc.Should().Be(expectedStrengthPerc);
        feedbackDetail.WeaknessPerc.Should().Be(expectedWeaknessPerc);
    }

    [Test, MoqInlineAutoData]
    public void Then_Employer_Feedback_Details_As_Expected_Overall_Tag(GetProviderQueryResponse response, string feedbackName, int strength, int weakness, int reviewCount, int stars)
    {
        response.AnnualApprenticeFeedbackDetails = null;
        response.AnnualEmployerFeedbackDetails = GetEmployerAnnualSummaries(feedbackName, strength, weakness, reviewCount, stars);

        var sut = (ProviderDetailsViewModel)response;
        sut.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, new DateTimeService().GetDateTime());

        var totalCount = strength + weakness;
        int expectedStrengthPerc = (int)((totalCount == 0) ? 0 : Math.Round(((double)(strength + 2) * 100) / totalCount));
        int expectedWeaknessPerc = 100 - expectedStrengthPerc;

        sut.FeedbackSurvey.FeedbackByYear.Count.Should().Be(6);
        var feedbackTab = sut.FeedbackSurvey.FeedbackByYear[5];
        feedbackTab.IsMostRecentYear.Should().Be(false);
        feedbackTab.EndYear.Should().Be(0);
        feedbackTab.StartYear.Should().Be(0);
        feedbackTab.Heading.Should().Be("Overall reviews");
        feedbackTab.SubHeading.Should().Be("1 August 2020 to 31 July 2024");
        feedbackTab.MainText.Should().Be(FeedbackSurveyViewModel.EmployerReviewsOverallText);
        feedbackTab.NoEmployerReviewsText.Should().Be(FeedbackSurveyViewModel.EmployersNoResultsPastTab);
        feedbackTab.ShowEmployerFeedbackStars.Should().Be(true);
        feedbackTab.ShowApprenticeFeedbackStars.Should().Be(false);
        feedbackTab.TimePeriod.Should().Be(TimePeriodAll);
        var feedbackDetail = feedbackTab.EmployerFeedbackDetails.ProviderAttributes[0];
        feedbackDetail.TotalCount.Should().Be(strength + weakness);
        feedbackDetail.Strength.Should().Be(strength + 2);
        feedbackDetail.Weakness.Should().Be(weakness - 2);
        feedbackDetail.StrengthPerc.Should().Be(expectedStrengthPerc);
        feedbackDetail.WeaknessPerc.Should().Be(expectedWeaknessPerc);
    }

    private static List<EmployerFeedbackAnnualSummaries> GetEmployerAnnualSummaries(string feedbackName, int strength, int weakness, int reviewCount, int stars)
    {
        var annualSummaryItem = new AnnualSummaryItem { Name = feedbackName, Strength = strength, Weakness = weakness };
        var annualSummaryItem2 = new AnnualSummaryItem { Name = feedbackName + "2", Strength = strength + 1, Weakness = weakness - 1 };
        var annualSummaryItem3 = new AnnualSummaryItem { Name = feedbackName + "3", Strength = strength + 2, Weakness = weakness - 2 };

        var summaries = new List<EmployerFeedbackAnnualSummaries>
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
