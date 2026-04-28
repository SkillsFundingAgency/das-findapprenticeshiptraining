using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class StandardViewModelTests
{
    [InlineAutoData(LearningType.Apprenticeship, "govuk-tag--blue")]
    [InlineAutoData(LearningType.FoundationApprenticeship, "govuk-tag--pink")]
    [InlineAutoData(LearningType.ApprenticeshipUnit, "govuk-tag--purple")]
    public void LearningTypeTagClass_ForKnownType_ReturnsExpectedCssClass(
        LearningType learningType,
        string expectedCssClass,
        StandardModel standardModel,
        FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = 1;
        standardModel.LearningType = learningType;
        var sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        sut.LearningTypeTagClass.Should().Be(expectedCssClass);
    }

    [InlineAutoData(CourseType.Apprenticeship, false, true)]
    [InlineAutoData(CourseType.ShortCourse, true, false)]
    public void CourseTypeFlags_AreSetCorrectly(CourseType courseType, bool expectedShortCourseType, bool expectedApprenticeshipType, StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = 1;
        standardModel.CourseType = courseType;

        StandardViewModel sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        sut.IsShortCourseType.Should().Be(expectedShortCourseType);
        sut.IsApprenticeshipType.Should().Be(expectedApprenticeshipType);
    }

    [InlineAutoData(1, true)]
    [InlineAutoData(0, false)]
    public void HasProviderFlag_IsSetCorrectly(int providerCount, bool expectedValue, StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = 1;
        standardModel.ProvidersCount = providerCount;

        StandardViewModel sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        sut.HasProviders.Should().Be(expectedValue);
    }

    [Test, AutoData]
    public void LearningTypeTagClass_ForUnknownType_ReturnsEmptyString(StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = 1;
        standardModel.LearningType = (LearningType)999;
        var sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);
        sut.LearningTypeTagClass.Should().Be(string.Empty);
    }

    [Test, AutoData]
    public void ImplicitOperator_FromStandardModel_MapsAllFields(StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = 1;
        StandardViewModel sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        using (new AssertionScope())
        {
            sut.Ordering.Should().Be(standardModel.Ordering);
            sut.StandardUId.Should().Be(standardModel.StandardUId);
            sut.IfateReferenceNumber.Should().Be(standardModel.IfateReferenceNumber);
            sut.LarsCode.Should().Be(standardModel.LarsCode);
            sut.SearchScore.Should().Be(standardModel.SearchScore);
            sut.ProvidersCount.Should().Be(standardModel.ProvidersCount);
            sut.TotalProvidersCount.Should().Be(standardModel.TotalProvidersCount);
            sut.Title.Should().Be(standardModel.Title);
            sut.Level.Should().Be(standardModel.Level);
            sut.OverviewOfRole.Should().Be(standardModel.OverviewOfRole);
            sut.Keywords.Should().Be(standardModel.Keywords);
            sut.Route.Should().Be(standardModel.Route);
            sut.RouteCode.Should().Be(standardModel.RouteCode);
            sut.TypicalDuration.Should().Be(standardModel.TypicalDuration);
            sut.LearningType.Should().Be(standardModel.LearningType);
            sut.CourseType.Should().Be(standardModel.CourseType);
        }
    }

    [Test, AutoData]
    public void MaxFunding_ReturnsFormatedCurrency(StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = 1;
        standardModel.MaxFunding = 10000;

        StandardViewModel sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.That(sut.MaxFunding, Is.EqualTo("£10,000"));
    }

    [InlineAutoData(1, "1 - equal to GCSE")]
    [InlineAutoData(2, "2 - equal to A Level")]
    public void LevelName_WithExistingLevel_ReturnsFormattedName(int level, string expectedLevelName, StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.Level = level;

        StandardViewModel sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.That(sut.LevelName, Is.EqualTo(expectedLevelName));
    }

    [InlineAutoData(0, "")]
    [InlineAutoData(1, "View 1 training provider for this course")]
    [InlineAutoData(2, "View 2 training providers for this course")]
    public void FindProvidersUrlDescription_WihtoutLocation_ReturnsCorrectDescription(int providerCount, string expectedUrlDescription, StandardModel standardModel, FindApprenticeshipTrainingWeb config)
    {
        standardModel.ProvidersCount = providerCount;
        standardModel.Level = 1;
        StandardViewModel sut = new StandardViewModel(standardModel, string.Empty, string.Empty, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.That(sut.FindProvidersUrlDescription, Is.EqualTo(expectedUrlDescription));
    }

    [InlineAutoData(1, 10, "View 1 training provider within 10 miles")]
    [InlineAutoData(2, 20, "View 2 training providers within 20 miles")]
    public void FindProvidersUrlDescription_WihtLocation_ReturnsCorrectDescription(int providerCount, int distance, string expectedUrlDescription, StandardModel standardModel, FindApprenticeshipTrainingWeb config, string location)
    {
        standardModel.Level = Levels[0].Code;
        standardModel.ProvidersCount = providerCount;
        StandardViewModel sut = new StandardViewModel(standardModel, location, distance.ToString(), config, Mock.Of<IUrlHelper>(), Levels);

        Assert.That(sut.FindProvidersUrlDescription, Is.EqualTo(expectedUrlDescription));
    }

    [Test]
    public void FindProvidersUrl_WhenNoProviders_ReturnsEmpty()
    {
        // Arrange
        var standard = new StandardModel
        {
            LarsCode = "123",
            ProvidersCount = 0,
            Level = 1
        };

        var config = new FindApprenticeshipTrainingWeb();
        var urlHelperMock = new Mock<IUrlHelper>(MockBehavior.Strict);

        // Act
        var sut = new StandardViewModel(standard, location: string.Empty, distance: string.Empty, findApprenticeshipTrainingWebConfiguration: config, urlHelper: urlHelperMock.Object, Levels);

        // Assert
        Assert.That(sut.FindProvidersUrl, Is.EqualTo(string.Empty));
        urlHelperMock.Verify(x => x.RouteUrl(It.IsAny<UrlRouteContext>()), Times.Never);
    }

    [InlineAutoData(DistanceService.ACROSS_ENGLAND_FILTER_VALUE, DistanceService.ACROSS_ENGLAND_FILTER_VALUE)]
    [InlineAutoData("10", "10")]
    public void FindProvidersUrl_GeneratesUrl(string distance, string expectedValue, StandardModel standard, FindApprenticeshipTrainingWeb config)
    {
        // Arrange
        standard.LarsCode = "200";
        standard.ProvidersCount = 3;
        standard.Level = Levels[0].Code;
        var urlHelperMock = new Mock<IUrlHelper>();
        UrlRouteContext captured = null;

        urlHelperMock
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Callback<UrlRouteContext>(c => captured = c)
            .Returns("/courses/200/providers");

        // Act
        var sut = new StandardViewModel(
            standard,
            location: string.Empty,
            distance: distance,
            findApprenticeshipTrainingWebConfiguration: config,
            urlHelper: urlHelperMock.Object,
            levels: Levels);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.FindProvidersUrl, Is.EqualTo("/courses/200/providers"));
            Assert.That(captured, Is.Not.Null, "UrlRouteContext was not provided to IUrlHelper.RouteUrl");

            var values = new RouteValueDictionary(captured.Values);
            Assert.That(captured.RouteName, Is.EqualTo(RouteNames.CourseProviders));
            Assert.That(values["larsCode"]?.ToString(), Is.EqualTo(standard.LarsCode));
            Assert.That(values["location"]?.ToString(), Is.EqualTo(string.Empty));
            Assert.That(values["distance"]?.ToString(), Is.EqualTo(expectedValue));
        });
    }

    [InlineAutoData("MK4", DistanceService.ACROSS_ENGLAND_FILTER_VALUE, 10, "View 10 training providers for this course")]
    [InlineAutoData("MK4", DistanceService.ACROSS_ENGLAND_FILTER_VALUE, 1, "View 1 training provider for this course")]
    [InlineAutoData("", DistanceService.ACROSS_ENGLAND_FILTER_VALUE, 10, "View 10 training providers for this course")]
    [InlineAutoData("", DistanceService.ACROSS_ENGLAND_FILTER_VALUE, 1, "View 1 training provider for this course")]
    [InlineAutoData("MK4", "10", 10, "View 10 training providers within 10 miles")]
    [InlineAutoData("MK4", "20", 1, "View 1 training provider within 20 miles")]
    public void FindProvidersUrlDescription_ReturnsCorrectDescription(string location, string distance, int providerCount, string expectedDescription, StandardModel standard, FindApprenticeshipTrainingWeb config)
    {
        standard.ProvidersCount = providerCount;
        standard.Level = Levels[0].Code;

        var sut = new StandardViewModel(standard, location, distance, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.That(sut.FindProvidersUrlDescription, Is.EqualTo(expectedDescription));
    }

    [InlineAutoData(CourseType.ShortCourse, 0)] //Does not apply to short courses
    [InlineAutoData(CourseType.Apprenticeship, 1)] //Does not apply to apprenticeship with providers
    public void RequestApprenticeshipTrainingUrl_ReturnsEmpty(CourseType courseType, int providerCount, string location, string distance, StandardModel standard)
    {
        standard.Level = Levels[0].Code;
        standard.CourseType = courseType;
        standard.ProvidersCount = providerCount;
        var config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://localhost",
            EmployerAccountsUrl = "https://accounts"
        };

        var sut = new StandardViewModel(standard, location, distance, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.That(sut.RequestApprenticeshipTrainingUrl, Is.Empty);
    }

    [AutoData]
    public void RequestApprenticeshipTrainingUrl_ReturnsUrl(string distance, StandardModel standard)
    {
        string location = "M60 7RA";
        standard.LarsCode = "123";
        standard.Level = Levels[0].Code;
        standard.CourseType = CourseType.Apprenticeship;
        standard.ProvidersCount = 0;
        var config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://localhost",
            EmployerAccountsUrl = "https://accounts"
        };

        var sut = new StandardViewModel(standard, location, distance, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.Multiple(() =>
        {
            Assert.That(sut.RequestApprenticeshipTrainingUrl, Does.StartWith("https://accounts/service/?redirectUri="));
            Assert.That(Uri.UnescapeDataString(sut.RequestApprenticeshipTrainingUrl), Does.Contain("standardId=123"));
            Assert.That(Uri.UnescapeDataString(sut.RequestApprenticeshipTrainingUrl), Does.Contain("location=M60 7RA"));
        });
    }

    [AutoData]
    public void GenerateStandardRouteValues_WithLocation_IncludesLocationAndDistance(StandardModel standard, FindApprenticeshipTrainingWeb config, string location, string distance)
    {
        standard.Level = Levels[0].Code;

        var sut = new StandardViewModel(standard, location, distance, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.Multiple(() =>
        {
            Assert.That(sut.CourseDetailsRouteValues["larsCode"], Is.EqualTo(standard.LarsCode));
            Assert.That(sut.CourseDetailsRouteValues.ContainsKey("location"), Is.True);
            Assert.That(sut.CourseDetailsRouteValues.ContainsKey("distance"), Is.True);
            Assert.That(sut.CourseDetailsRouteValues["location"], Is.EqualTo(location));
            Assert.That(sut.CourseDetailsRouteValues["distance"], Is.EqualTo(distance));
        });
    }

    [AutoData]
    public void GenerateStandardRouteValues_WithOutLocation_ExcludesLocationAndDistance(StandardModel standard, FindApprenticeshipTrainingWeb config, string distance)
    {
        standard.Level = Levels[0].Code;
        var location = string.Empty;

        var sut = new StandardViewModel(standard, location, distance, config, Mock.Of<IUrlHelper>(), Levels);

        Assert.Multiple(() =>
        {
            Assert.That(sut.CourseDetailsRouteValues["larsCode"], Is.EqualTo(standard.LarsCode));
            Assert.That(sut.CourseDetailsRouteValues.ContainsKey("location"), Is.False);
            Assert.That(sut.CourseDetailsRouteValues.ContainsKey("distance"), Is.False);
        });
    }


    private static List<LevelViewModel> Levels =>
    [
        new(new Level{ Code = 1, Name = "GCSE" }, []),
        new(new Level{ Code = 2, Name = "A Level" }, []),
    ];
}
