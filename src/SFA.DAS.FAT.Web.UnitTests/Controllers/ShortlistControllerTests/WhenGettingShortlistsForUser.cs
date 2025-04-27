using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests;

public class WhenGettingShortlistsForUser
{
    private const string RatUrl = "https://rat.test.apprenticeships.org.uk";
    private Mock<ICookieStorageService<ShortlistCookieItem>> _shortlistCookieServiceMock;
    private Mock<IMediator> _mediatorMock;
    private Mock<ITempDataDictionary> _tempDataMock;
    private Mock<IDataProtectionProvider> _dataProtectionProviderMock;
    private Mock<IRequestApprenticeshipTrainingService> _ratServiceMock;
    private Mock<ISessionService> _sessionServiceMock;
    private ShortlistController _sut;
    private ShortlistCookieItem _shortlistCookieItem;
    private GetShortlistsForUserResponse _mediatorResponse;

    [SetUp]
    public void Setup()
    {
        _shortlistCookieItem = new() { ShortlistUserId = Guid.NewGuid() };
        _shortlistCookieServiceMock = new();
        _mediatorMock = new();
        _tempDataMock = new();
        _dataProtectionProviderMock = new();
        _ratServiceMock = new();
        _sessionServiceMock = new();

        _sut = new(_mediatorMock.Object, _shortlistCookieServiceMock.Object, _dataProtectionProviderMock.Object, _ratServiceMock.Object, _sessionServiceMock.Object);
        _sut.TempData = _tempDataMock.Object;

        _ratServiceMock.Setup(x => x.GetRequestApprenticeshipTrainingUrl(It.IsAny<int>(), It.IsAny<EntryPoint>(), It.IsAny<string>())).Returns(RatUrl);

        _shortlistCookieServiceMock.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns(_shortlistCookieItem);

        SetupMediatorResponse();
    }

    private void SetupMediatorResponse()
    {
        _mediatorResponse = new();
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetShortlistsForUserQuery>(), default)).ReturnsAsync(_mediatorResponse);

        _mediatorResponse.ShortlistsExpiryDate = DateTime.Now.AddDays(ShortlistController.ShortlistExpiryInDays);
        _mediatorResponse.QarPeriod = "0102";
        _mediatorResponse.ReviewPeriod = "0304";
        _mediatorResponse.Courses = new List<ShortlistCourseModel>
        {
            new ShortlistCourseModel
            {
                LarsCode = 1,
                StandardName = "Course 1",
                Locations = new List<ShortlistLocationModel>
                {
                    new ShortlistLocationModel
                    {
                        LocationDescription = "Location 1",
                        Providers = new List<ShortlistProviderModel>
                        {
                            new ShortlistProviderModel
                            {
                                ShortlistId = Guid.NewGuid(),
                                Ukprn = 10012002,
                                ProviderName = "Provider 1",
                                AtEmployer = true,
                                HasBlockRelease = true,
                                BlockReleaseDistance = 10,
                                BlockReleaseCount = 2,
                                HasDayRelease = true,
                                DayReleaseDistance = 20,
                                DayReleaseCount = 1,
                                Email = "sample@test.com",
                                Phone = "0123456789",
                                Website = "https://www.test.com",
                                Leavers = "100",
                                QarPeriod = "0102",
                                ReviewPeriod = "0304",
                                AchievementRate = "80",
                                EmployerReviews = "4.5",
                                EmployerStars = "4",
                                EmployerRating = "Good",
                                ApprenticeReviews = "4.5",
                                ApprenticeStars = "4",
                                ApprenticeRating = "Good",
                            }
                        }
                    }
                }
            }
        };
    }

    [TestCase(null, false)]
    [TestCase(49, false)]
    [TestCase(50, true)]
    public async Task ThenSetsHasMaxedOutShortlistsFlag(int? count, bool expected)
    {
        ShortlistsCount shortlistCount = count is null ? null : new() { Count = count.GetValueOrDefault() };
        _sessionServiceMock.Setup(x => x.Get<ShortlistsCount>()).Returns(shortlistCount);
        //Act
        var result = await _sut.Index();
        //Assert
        result.As<ViewResult>().Model.As<ShortlistsViewModel>().HasMaxedOutShortlists.Should().Be(expected);
    }

    [Test]
    public async Task ThenInvokesMediatorToGetShortlistsForUser()
    {
        //Act
        await _sut.Index();
        //Assert
        _shortlistCookieServiceMock.Verify(x => x.Get(Constants.ShortlistCookieName), Times.Once);
    }

    [Test, AutoData]
    public async Task ThenGetsRemovedProviderNameFromTempData(string providerName)
    {
        _tempDataMock.SetupGet(t => t[ShortlistController.RemovedProviderNameTempDataKey]).Returns(providerName);
        //Act
        var result = await _sut.Index();
        //Assert
        _tempDataMock.VerifyGet(t => t[ShortlistController.RemovedProviderNameTempDataKey], Times.Once);
        result.As<ViewResult>().Model.As<ShortlistsViewModel>().RemovedProviderName.Should().Be(providerName);
    }

    [Test]
    public async Task ThenReturnsEmptyRemovedProviderName()
    {
        _tempDataMock.SetupGet(t => t[ShortlistController.RemovedProviderNameTempDataKey]).Returns(null);
        //Act
        var result = await _sut.Index();
        //Assert
        _tempDataMock.VerifyGet(t => t[ShortlistController.RemovedProviderNameTempDataKey], Times.Once);
        result.As<ViewResult>().Model.As<ShortlistsViewModel>().RemovedProviderName.Should().BeNull();
    }

    [Test, AutoData]
    public async Task ThenSetsShortlistExpiryDate(DateTime expiryDate)
    {
        //Arrange
        _mediatorResponse.ShortlistsExpiryDate = expiryDate;
        var expected = expiryDate.ToString("d MMMM yyyy");
        //Act
        var result = await _sut.Index();
        //Assert
        result.As<ViewResult>().Model.As<ShortlistsViewModel>().ExpiryDateText.Should().Be(expected);
    }

    [Test]
    public async Task ThenAddsCourses()
    {
        //Arrange
        var expected = _mediatorResponse.Courses[0];
        //Act
        var result = await _sut.Index();
        //Assert
        var actual = result.As<ViewResult>().Model.As<ShortlistsViewModel>().Courses[0];
        actual.LarsCode.Should().Be(expected.LarsCode);
        actual.CourseTitle.Should().Be(expected.StandardName);
    }

    [Test]
    public async Task ThenAddsLocationForEachCourse()
    {
        //Arrange
        var expected = _mediatorResponse.Courses[0].Locations[0];
        //Act
        var result = await _sut.Index();
        //Assert
        using (new AssertionScope())
        {
            var actual = result.As<ViewResult>().Model.As<ShortlistsViewModel>().Courses[0].Locations[0];
            actual.Description.Should().Be(expected.LocationDescription);
            actual.QarPeriod.Should().Be($"20{_mediatorResponse.QarPeriod.AsSpan(0, 2)} to 20{_mediatorResponse.QarPeriod.AsSpan(2, 2)}");
            actual.ReviewPeriod.Should().Be($"20{_mediatorResponse.ReviewPeriod.AsSpan(0, 2)} to 20{_mediatorResponse.ReviewPeriod.AsSpan(2, 2)}");
            actual.RequestApprenticeshipTraining.CourseTitle.Should().Be(_mediatorResponse.Courses[0].StandardName);
            actual.RequestApprenticeshipTraining.Url.Should().Be(RatUrl);
        }
    }

    [Test]
    public async Task ThenAddsProvidersForEachLocation()
    {
        //Arrange
        var expectedCourse = _mediatorResponse.Courses[0];
        var expectedProvider = expectedCourse.Locations[0].Providers[0];
        //Act
        var result = await _sut.Index();
        //Assert
        using (new AssertionScope())
        {
            var actualProvider = result.As<ViewResult>().Model.As<ShortlistsViewModel>().Courses[0].Locations[0].Providers[0];
            actualProvider.LarsCode.Should().Be(expectedCourse.LarsCode);
            actualProvider.LocationDescription.Should().Be(expectedCourse.Locations[0].LocationDescription);
            actualProvider.ShortlistId.Should().Be(expectedProvider.ShortlistId);
            actualProvider.Ukprn.Should().Be(expectedProvider.Ukprn);
            actualProvider.ProviderName.Should().Be(expectedProvider.ProviderName);
            actualProvider.AtEmployer.Should().Be(expectedProvider.AtEmployer);
            actualProvider.HasBlockRelease.Should().Be(expectedProvider.HasBlockRelease);
            actualProvider.BlockReleaseDistance.Should().Be(expectedProvider.BlockReleaseDistance);
            actualProvider.HasMultipleBlockRelease.Should().BeTrue();
            actualProvider.HasDayRelease.Should().Be(expectedProvider.HasDayRelease);
            actualProvider.DayReleaseDistance.Should().Be(expectedProvider.DayReleaseDistance);
            actualProvider.HasMultipleDayRelease.Should().BeFalse();
            actualProvider.Email.Should().Be(expectedProvider.Email);
            actualProvider.Phone.Should().Be(expectedProvider.Phone);
            actualProvider.Website.Should().Be(expectedProvider.Website);
            actualProvider.Leavers.Should().Be(expectedProvider.Leavers);
            actualProvider.AchievementRate.Should().Be(expectedProvider.AchievementRate);
            actualProvider.EmployerReviews.ProviderRatingType.Should().Be(ProviderRatingType.Employer);
            actualProvider.EmployerReviews.Reviews.Should().Be(expectedProvider.EmployerReviews);
            actualProvider.EmployerReviews.Stars.Should().Be(expectedProvider.EmployerStars);
            actualProvider.EmployerReviews.ProviderRating.Should().Be(Enum.Parse<ProviderRating>(expectedProvider.EmployerRating));
            actualProvider.ApprenticeReviews.ProviderRatingType.Should().Be(ProviderRatingType.Apprentice);
            actualProvider.ApprenticeReviews.Reviews.Should().Be(expectedProvider.ApprenticeReviews);
            actualProvider.ApprenticeReviews.Stars.Should().Be(expectedProvider.ApprenticeStars);
            actualProvider.ApprenticeReviews.ProviderRating.Should().Be(Enum.Parse<ProviderRating>(expectedProvider.ApprenticeRating));
        }
    }
}
