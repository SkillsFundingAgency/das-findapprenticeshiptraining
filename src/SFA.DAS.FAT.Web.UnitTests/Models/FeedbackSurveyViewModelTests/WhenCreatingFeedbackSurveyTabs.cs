using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;

namespace SFA.DAS.FAT.Web.UnitTests.Models.FeedbackSurveyViewModelTests;

public class WhenCreatingFeedbackSurveyTabs
{
    [TestCaseSource(nameof(_tabCases))]
    public void Then_The_Model_Is_Converted_From_Result_Correctly(DateTime dateToUse, int tab, string heading, string subHeading, string mainText, string timePeriod)
    {
        var sut = FeedbackSurveyViewModel.ProcessFeedbackDetails(new List<EmployerFeedbackAnnualSummaries>(),
            new List<ApprenticeFeedbackAnnualSummaries>(), dateToUse);

        var tabToCheck = tab - 1;
        sut.FeedbackByYear.Count.Should().Be(6);
        sut.FeedbackByYear[tabToCheck].Heading.Should().Be(heading);
        sut.FeedbackByYear[tabToCheck].SubHeading.Should().Be(subHeading);
        sut.FeedbackByYear[tabToCheck].MainText.Should().Be(mainText);
        sut.FeedbackByYear[tabToCheck].TimePeriod.Should().Be(timePeriod);
    }

    private static IEnumerable<object[]> _tabCases()
    {
        var NextYear = DateTime.UtcNow.AddYears(1).ToString("yy");
        var ThisYear = DateTime.UtcNow.ToString("yy");
        var LastYear = DateTime.UtcNow.AddYears(-1).ToString("yy");
        var TwoYearsAgo = DateTime.UtcNow.AddYears(-2).ToString("yy");
        var ThreeYearsAgo = DateTime.UtcNow.AddYears(-3).ToString("yy");
        var FourYearsAgo = DateTime.UtcNow.AddYears(-4).ToString("yy");
        var FiveYearsAgo = DateTime.UtcNow.AddYears(-5).ToString("yy");


        yield return
        [
            new DateTime(2025, 07, 31), 1, $"20{LastYear} to today", $"1 August 20{LastYear} to today",
            FeedbackSurveyViewModel.EmployerMostRecentReviewsText, $"AY{LastYear}{ThisYear}"
        ];
        yield return
        [
            new DateTime(2025, 07, 31), 2, $"20{TwoYearsAgo} to 20{LastYear}",
            $"1 August 20{TwoYearsAgo} to 31 July 20{LastYear}",
            FeedbackSurveyViewModel.AllCoursesDeliveredTextLastFullYear, $"AY{TwoYearsAgo}{LastYear}"
        ];
        yield return
        [
            new DateTime(2025, 07, 31), 3, $"20{ThreeYearsAgo} to 20{TwoYearsAgo}",
            $"1 August 20{ThreeYearsAgo} to 31 July 20{TwoYearsAgo}",
            FeedbackSurveyViewModel.AllCoursesDeliveredText, $"AY{ThreeYearsAgo}{TwoYearsAgo}"
        ];
        yield return
        [
            new DateTime(2025, 07, 31), 4, $"20{FourYearsAgo} to 20{ThreeYearsAgo}",
            $"1 August 20{FourYearsAgo} to 31 July 20{ThreeYearsAgo}",
            FeedbackSurveyViewModel.AllCoursesDeliveredText, $"AY{FourYearsAgo}{ThreeYearsAgo}"
        ];
        yield return
        [
            new DateTime(2025, 07, 31), 5, $"20{FiveYearsAgo} to 20{FourYearsAgo}",
            $"1 August 20{FiveYearsAgo} to 31 July 20{FourYearsAgo}",
            FeedbackSurveyViewModel.AllCoursesDeliveredText, $"AY{FiveYearsAgo}{FourYearsAgo}"
        ];
        yield return
        [
            new DateTime(2025, 07, 31), 6, "Overall reviews", $"1 August 20{FiveYearsAgo} to today",
            FeedbackSurveyViewModel.EmployerReviewsOverallText, "All"
        ];
        yield return
        [
            new DateTime(2025, 08, 01), 1, $"20{ThisYear} to today", $"1 August 20{ThisYear} to today",
            FeedbackSurveyViewModel.EmployerMostRecentReviewsText, $"AY{ThisYear}{NextYear}"
        ];
        yield return
        [
            new DateTime(2025, 08, 01), 2, $"20{LastYear} to 20{ThisYear}",
            $"1 August 20{LastYear} to 31 July 20{ThisYear}",
            FeedbackSurveyViewModel.AllCoursesDeliveredTextLastFullYear, $"AY{LastYear}{ThisYear}"
        ];
        yield return
        [
            new DateTime(2025, 08, 01), 3, $"20{TwoYearsAgo} to 20{LastYear}",
            $"1 August 20{TwoYearsAgo} to 31 July 20{LastYear}", FeedbackSurveyViewModel.AllCoursesDeliveredText,
            $"AY{TwoYearsAgo}{LastYear}"
        ];
        yield return
        [
            new DateTime(2025, 08, 01), 4, $"20{ThreeYearsAgo} to 20{TwoYearsAgo}",
            $"1 August 20{ThreeYearsAgo} to 31 July 20{TwoYearsAgo}",
            FeedbackSurveyViewModel.AllCoursesDeliveredText, $"AY{ThreeYearsAgo}{TwoYearsAgo}"
        ];
        yield return
        [
            new DateTime(2025, 08, 01), 5, $"20{FourYearsAgo} to 20{ThreeYearsAgo}",
            $"1 August 20{FourYearsAgo} to 31 July 20{ThreeYearsAgo}",
            FeedbackSurveyViewModel.AllCoursesDeliveredText, $"AY{FourYearsAgo}{ThreeYearsAgo}"
        ];
        yield return
        [
            new DateTime(2025, 08, 01), 6, "Overall reviews", $"1 August 20{FourYearsAgo} to today",
            FeedbackSurveyViewModel.EmployerReviewsOverallText, "All"
        ];
    }
}
