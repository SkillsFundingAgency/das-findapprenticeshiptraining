using NUnit.Framework;
using Microsoft.Extensions.Options;
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

        Assert.That(sut.TypicalJobTitlesArray, Is.EqualTo(new[] { "Software Engineer", "Construction Worker" }));
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
}
