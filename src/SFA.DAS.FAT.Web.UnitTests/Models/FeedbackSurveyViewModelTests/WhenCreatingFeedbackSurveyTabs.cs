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

    private static object[] _tabCases =
    {
        new object[] { new DateTime(2025,07,31),1, "2024 to today","1 August 2024 to today", FeedbackSurveyViewModel.EmployerMostRecentReviewsText, "AY2425" },
        new object[] { new DateTime(2025,07,31),2, "2023 to 2024","1 August 2023 to 31 July 2024", FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2324" },
        new object[] { new DateTime(2025,07,31),3, "2022 to 2023","1 August 2022 to 31 July 2023", FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2223" },
        new object[] { new DateTime(2025,07,31),4, "2021 to 2022","1 August 2021 to 31 July 2022", FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2122" },
        new object[] { new DateTime(2025,07,31),5, "2020 to 2021","1 August 2020 to 31 July 2021", FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2021" },
        new object[] { new DateTime(2025,07,31),6, "Overall reviews","1 August 2020 to 31 July 2024",  FeedbackSurveyViewModel.EmployerReviewsOverallText, "All" },
        new object[] { new DateTime(2025,08,01),1, "2025 to today","1 August 2025 to today", FeedbackSurveyViewModel.EmployerMostRecentReviewsText, "AY2526" },
        new object[] { new DateTime(2025,08,01),2, "2024 to 2025","1 August 2024 to 31 July 2025",FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2425"  },
        new object[] { new DateTime(2025,08,01),3, "2023 to 2024","1 August 2023 to 31 July 2024",FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2324"  },
        new object[] { new DateTime(2025,08,01),4, "2022 to 2023","1 August 2022 to 31 July 2023",FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2223"  },
        new object[] { new DateTime(2025,08,01),5, "2021 to 2022","1 August 2021 to 31 July 2022",FeedbackSurveyViewModel.AllCoursesDeliveredText, "AY2122"  },
        new object[] { new DateTime(2025,08,01),6, "Overall reviews","1 August 2021 to 31 July 2025", FeedbackSurveyViewModel.EmployerReviewsOverallText, "All" }
    };
}
