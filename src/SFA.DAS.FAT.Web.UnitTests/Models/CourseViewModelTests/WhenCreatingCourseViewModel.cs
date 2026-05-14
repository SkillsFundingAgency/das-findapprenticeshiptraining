using Microsoft.Extensions.Options;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseViewModelTests;

public sealed class WhenCreatingCourseViewModel
{
    [TestCase(LearningType.Apprenticeship, "govuk-tag--blue")]
    [TestCase(LearningType.FoundationApprenticeship, "govuk-tag--pink")]
    [TestCase(LearningType.ApprenticeshipUnit, "govuk-tag--purple")]
    [TestCase((LearningType)999, "")]
    public void LearningTypeTagClass_LearningTypeSet_ReturnsExpectedTagClass(LearningType learningType, string expected)
    {
        var sut = new CourseViewModel
        {
            LearningType = learningType
        };

        Assert.That(sut.LearningTypeTagClass, Is.EqualTo(expected));
    }

    [Test]
    public void LevelEquivalentToDisplayText_WithLevelsCountLessThanOne_ReturnsEmpty()
    {
        var sut = new CourseViewModel() { Level = 1 };
        Assert.That(sut.LevelEquivalentToDisplayText, Is.Empty);
    }

    [Test]
    public void LevelEquivalentToDisplayText_WhenLevelNotFound_ReturnsEmpty()
    {
        var sut = new CourseViewModel()
        {
            Level = 1,
            Levels = new List<Level>() { new Level() { Code = 2, Name = "Level 2" } }
        };

        Assert.That(sut.LevelEquivalentToDisplayText, Is.Empty);
    }

    [Test]
    public void LevelEquivalentToDisplayText_WhenLevelFound_ReturnsEqualToLevelName()
    {
        var sut = new CourseViewModel()
        {
            Level = 2,
            Levels = new List<Level>() { new Level() { Code = 2, Name = "Level 2" } }
        };

        Assert.That(sut.LevelEquivalentToDisplayText, Is.EqualTo("Equal to Level 2"));
    }

    [Test]
    public void TypicalJobTitlesArray_WhenEmpty_ReturnsEmptyArray()
    {
        var sut = new CourseViewModel()
        {
            TypicalJobTitles = string.Empty
        };

        Assert.That(sut.TypicalJobTitlesArray, Is.Empty);
    }

    [Test]
    public void TypicalJobTitlesArray_WhenPopulated_ReturnsSplitArray()
    {
        var sut = new CourseViewModel()
        {
            TypicalJobTitles = "Software Engineer|Construction Worker"
        };

        Assert.That(sut.TypicalJobTitlesArray, Is.EqualTo(["Software Engineer", "Construction Worker"]));
    }

    [Test]
    public void ProviderCountDisplayMessage_WithLocationAndNoProviders_ReturnsZeroProviderWithinDistanceMessage()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1"
        };

        var sut = model.ProviderCountDisplayMessage;

        Assert.That(sut, Is.EqualTo(CourseViewModel.ZeroProvidersWithinDistanceMessage));
    }

    [Test]
    public void ProviderCountDisplayMessage_WithLocationAndOneProvider_ReturnsSingleProviderWithinDistanceMessage()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1",
            ProvidersCountWithinDistance = 1
        };

        var sut = model.ProviderCountDisplayMessage;

        Assert.That(sut, Is.EqualTo(CourseViewModel.SingleProviderWithinDistanceMessage));
    }

    [Test]
    public void ProviderCountDisplayMessage_WithLocationAndMultipleProviders_ReturnsMultipleProviderWithinDistanceMessage()
    {
        var model = new CourseViewModel()
        {
            Location = "SW1",
            ProvidersCountWithinDistance = 2
        };

        var sut = model.ProviderCountDisplayMessage;

        Assert.That(sut, Is.EqualTo(CourseViewModel.MultipleProvidersWithinDistanceMessage.Replace("{{ProvidersCountWithinDistance}}", model.ProvidersCountWithinDistance.ToString())));
    }

    [Test]
    public void ProviderCountDisplayMessage_WithoutLocationAndOneProvider_ReturnsSingleProviderOutsideDistanceMessage()
    {
        var model = new CourseViewModel()
        {
            Location = string.Empty,
            TotalProvidersCount = 1
        };

        var sut = model.ProviderCountDisplayMessage;

        Assert.That(sut, Is.EqualTo(CourseViewModel.SingleProviderOutsideDistanceMessage));
    }

    [Test]
    public void ProviderCountDisplayMessage_WithoutLocationAndMultipleProviders_ReturnsMultipleProvidersOutsideDistanceMessage()
    {
        var model = new CourseViewModel()
        {
            Location = string.Empty,
            TotalProvidersCount = 2
        };

        var sut = model.ProviderCountDisplayMessage;

        Assert.That(sut, Is.EqualTo(CourseViewModel.MultipleProviderOutsideDistanceMessage.Replace("{{TotalProvidersCount}}", model.TotalProvidersCount.ToString())));
    }

    [Test]
    public void KnowledgeSkillsHeaderTextToDisplay_IsApprenticeshipUnit_ReturnsKnowledgeAndSkillsLearnersWillGain()
    {
        var sut = new CourseViewModel
        {
            IsApprenticeshipUnit = true
        };

        Assert.That(sut.KnowledgeSkillsHeaderTextToDisplay, Is.EqualTo(CourseViewModel.KnowledgeSkillsHeaderTextApprenticeshipUnit));
    }

    [Test]
    public void KnowledgeSkillsHeaderTextToDisplay_IsNotApprenticeshipUnit_ReturnsKnowledgeSkillsAndBehaviours()
    {
        var sut = new CourseViewModel
        {
            IsApprenticeshipUnit = false
        };

        Assert.That(sut.KnowledgeSkillsHeaderTextToDisplay, Is.EqualTo(CourseViewModel.KnowledgeSkillsHeaderText));
    }

    [Test]
    public void KnowledgeSkillsLinkTextToDisplay_IsApprenticeshipUnit_ReturnsViewKnowledgeAndSkills()
    {
        var sut = new CourseViewModel
        {
            IsApprenticeshipUnit = true
        };

        Assert.That(sut.KnowledgeSkillsLinkTextToDisplay, Is.EqualTo(CourseViewModel.KnowledgeSkillsLinkTextApprenticeshipUnit));
    }

    [Test]
    public void KnowledgeSkillsLinkTextToDisplay_IsNotApprenticeshipUnit_ReturnsViewKnowledgeSkillsAndBehaviours()
    {
        var sut = new CourseViewModel
        {
            IsApprenticeshipUnit = false
        };

        Assert.That(sut.KnowledgeSkillsLinkTextToDisplay, Is.EqualTo(CourseViewModel.KnowledgeSkillsLinkText));
    }

    [Test]
    public void MaximumFundingTextToDisplay_IsApprenticeshipUnit_ReturnsUnitFundingText()
    {
        var sut = new CourseViewModel
        {
            IsApprenticeshipUnit = true
        };

        Assert.That(sut.MaximumFundingTextToDisplay, Is.EqualTo(CourseViewModel.MaximumFundingTextApprenticeshipUnit));
    }

    [Test]
    public void MaximumFundingTextToDisplay_IsNotApprenticeshipUnit_ReturnsApprenticeshipFundingText()
    {
        var sut = new CourseViewModel
        {
            IsApprenticeshipUnit = false
        };

        Assert.That(sut.MaximumFundingTextToDisplay, Is.EqualTo(CourseViewModel.MaximumFundingText));
    }

    [Test]
    public void HasLocation_LocationIsWhiteSpace_ReturnsFalse()
    {
        var sut = new CourseViewModel
        {
            Location = "   "
        };

        Assert.That(sut.HasLocation, Is.False);
    }

    [Test]
    public void HasLocation_LocationIsNull_ReturnsFalse()
    {
        var sut = new CourseViewModel
        {
            Location = null
        };

        Assert.That(sut.HasLocation, Is.False);
    }

    [Test]
    public void HasLocation_LocationIsSet_ReturnsTrue()
    {
        var sut = new CourseViewModel
        {
            Location = "SW1"
        };

        Assert.That(sut.HasLocation, Is.True);
    }

    [Test]
    public void ApprenticeCanTravelDisplayMessage_WithDistanceAll_ReturnsAcrossEnglandDisplayText()
    {
        var sut = new CourseViewModel()
        {
            Distance = "All"
        };

        Assert.That(sut.ApprenticeCanTravelDisplayMessage, Is.EqualTo(DistanceService.AcrossEnglandDisplayText));
    }

    [Test]
    public void ApprenticeCanTravelDisplayMessage_WithDistanceSet_ReturnsMilesDisplayText()
    {
        var sut = new CourseViewModel()
        {
            Distance = "10"
        };

        Assert.That(sut.ApprenticeCanTravelDisplayMessage, Is.EqualTo($"{sut.Distance} miles"));
    }

    [Test, MoqAutoData]
    public void HelpFindingCourseUrl_WithLocationSet_ReturnsRequestApprenticeshipTrainingUrlWithLocation(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var _sut = new CourseViewModel()
        {
            LarsCode = "1",
            Location = "SW1",
            ConfigOptions = Options.Create(findApprenticeshipTrainingWebConfiguration)
        };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={_sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + "&location=SW1")}";

        var result = _sut.HelpFindingCourseUrl;
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test, MoqAutoData]
    public void HelpFindingCourseUrl_WithoutLocationSet_ReturnsRequestApprenticeshipTrainingUrlWithoutLocation(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var _sut = new CourseViewModel()
        {
            LarsCode = "1",
            Location = string.Empty,
            ConfigOptions = Options.Create(findApprenticeshipTrainingWebConfiguration)
        };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={_sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri)}";

        var result = _sut.HelpFindingCourseUrl;
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test, MoqAutoData]
    public void HelpFindingCourseUrl_LocationIsWhitespace_ReturnsRequestApprenticeshipTrainingUrlWithoutLocation(
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        var sut = new CourseViewModel()
        {
            LarsCode = "1",
            Location = "   ",
            ConfigOptions = Options.Create(findApprenticeshipTrainingWebConfiguration)
        };

        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={sut.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri)}";

        var result = sut.HelpFindingCourseUrl;
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test]
    public void ProviderCountDisplayMessage_LocationIsWhitespace_ReturnsMultipleProvidersOutsideDistanceMessage()
    {
        var sut = new CourseViewModel
        {
            Location = "   ",
            TotalProvidersCount = 3
        };

        var result = sut.ProviderCountDisplayMessage;

        Assert.That(result, Is.EqualTo(CourseViewModel.MultipleProviderOutsideDistanceMessage.Replace("{{TotalProvidersCount}}", "3")));
    }

    [TestCase(0, "£0")]
    [TestCase(1000, "£1,000")]
    [TestCase(123456, "£123,456")]
    public void MaxFundingDisplayValue_MaxFundingSet_ReturnsCurrencyFormattedValue(int maxFunding, string expected)
    {
        var sut = new CourseViewModel
        {
            MaxFunding = maxFunding
        };

        Assert.That(sut.MaxFundingDisplayValue, Is.EqualTo(expected));
    }

    [TestCase(0, "£0")]
    [TestCase(3000, "£3,000")]
    [TestCase(999999, "£999,999")]
    public void IncentivePaymentDisplayValue_IncentivePaymentSet_ReturnsCurrencyFormattedValue(int incentivePayment, string expected)
    {
        var sut = new CourseViewModel
        {
            IncentivePayment = incentivePayment
        };

        Assert.That(sut.IncentivePaymentDisplayValue, Is.EqualTo(expected));
    }

    [Test]
    public void CourseInformationSummary_CourseViewModelHasValues_ReturnsMappedViewModel()
    {
        var ksbDetails = new List<KsbGroup> { new() { Type = KsbType.Knowledge, Details = new List<string> { "detail" } } };
        var relatedOccupations = new List<RelatedOccupation> { new() { Title = "Occupation", Level = 2 } };

        var sut = new CourseViewModel
        {
            LarsCode = "123",
            TitleAndLevel = "Title (level 2)",
            OverviewOfRole = "Overview",
            IsFoundationApprenticeship = true,
            IsApprenticeshipUnit = false,
            IncentivePayment = 2000,
            KsbDetails = ksbDetails,
            Route = "Health",
            Level = 2,
            Levels = new List<Level> { new() { Code = 2, Name = "Level 2" } },
            LearningType = LearningType.Apprenticeship,
            TypicalDuration = 18,
            MaxFunding = 12000,
            IsApprenticeship = true,
            TypicalJobTitles = "Job A|Job B",
            RelatedOccupations = relatedOccupations,
            StandardPageUrl = "https://example.com/course"
        };

        var result = sut.CourseInformationSummary;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.LarsCode, Is.EqualTo(sut.LarsCode));
            Assert.That(result.TitleAndLevel, Is.EqualTo(sut.TitleAndLevel));
            Assert.That(result.OverviewOfRole, Is.EqualTo(sut.OverviewOfRole));
            Assert.That(result.IsFoundationApprenticeship, Is.EqualTo(sut.IsFoundationApprenticeship));
            Assert.That(result.IsApprenticeshipUnit, Is.EqualTo(sut.IsApprenticeshipUnit));
            Assert.That(result.IncentivePaymentDisplayValue, Is.EqualTo(sut.IncentivePaymentDisplayValue));
            Assert.That(result.KnowledgeSkillsHeaderTextToDisplay, Is.EqualTo(sut.KnowledgeSkillsHeaderTextToDisplay));
            Assert.That(result.KnowledgeSkillsLinkTextToDisplay, Is.EqualTo(sut.KnowledgeSkillsLinkTextToDisplay));
            Assert.That(result.KsbDetails, Is.EqualTo(sut.KsbDetails));
            Assert.That(result.Route, Is.EqualTo(sut.Route));
            Assert.That(result.Level, Is.EqualTo(sut.Level));
            Assert.That(result.LevelEquivalentToDisplayText, Is.EqualTo(sut.LevelEquivalentToDisplayText));
            Assert.That(result.LearningType, Is.EqualTo(sut.LearningType));
            Assert.That(result.TypicalDuration, Is.EqualTo(sut.TypicalDuration));
            Assert.That(result.MaxFundingDisplayValue, Is.EqualTo(sut.MaxFundingDisplayValue));
            Assert.That(result.MaximumFundingTextToDisplay, Is.EqualTo(sut.MaximumFundingTextToDisplay));
            Assert.That(result.IsApprenticeship, Is.EqualTo(sut.IsApprenticeship));
            Assert.That(result.TypicalJobTitlesArray, Is.EqualTo(sut.TypicalJobTitlesArray));
            Assert.That(result.RelatedOccupations, Is.EqualTo(sut.RelatedOccupations));
            Assert.That(result.StandardPageUrl, Is.EqualTo(sut.StandardPageUrl));
        }
    }

    [Test]
    public void CourseProviderAvailability_CourseViewModelHasValues_ReturnsMappedViewModel()
    {
        var sut = new CourseViewModel
        {
            TotalProvidersCount = 2,
            IsShortCourseType = true,
            TitleAndLevel = "Title (level 2)",
            LarsCode = "123",
            Location = "Leeds",
            Distance = "All",
            ConfigOptions = Options.Create(new FindApprenticeshipTrainingWeb
            {
                EmployerAccountsUrl = "https://accounts.example",
                RequestApprenticeshipTrainingUrl = "https://rat.example"
            })
        };

        var result = sut.CourseProviderAvailability;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TotalProvidersCount, Is.EqualTo(sut.TotalProvidersCount));
            Assert.That(result.IsShortCourseType, Is.EqualTo(sut.IsShortCourseType));
            Assert.That(result.TitleAndLevel, Is.EqualTo(sut.TitleAndLevel));
            Assert.That(result.HelpFindingCourseUrl, Is.EqualTo(sut.HelpFindingCourseUrl));
            Assert.That(result.ProviderCountDisplayMessage, Is.EqualTo(sut.ProviderCountDisplayMessage));
            Assert.That(result.HasLocation, Is.EqualTo(sut.HasLocation));
            Assert.That(result.LarsCode, Is.EqualTo(sut.LarsCode));
            Assert.That(result.Location, Is.EqualTo(sut.Location));
            Assert.That(result.ApprenticeCanTravelDisplayMessage, Is.EqualTo(sut.ApprenticeCanTravelDisplayMessage));
            Assert.That(result.Distance, Is.EqualTo(sut.Distance));
        }
    }
}
