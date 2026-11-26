using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Helpers;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.FAT.Web.Validators;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CourseProvidersControllerTests;

public class CourseProvidersControllerCourseProvidersTests
{
    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndRequestIsValid_ThenReturnsViewWithCorrectData(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        Guid shortlistUserId,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {

        //Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        var distance = DistanceService.DefaultDistance;

        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;


        var qarStart = "21";
        var qarEnd = "22";
        var reviewStart = "22";
        var reviewEnd = "23";
        var qarPeriod = $"{qarStart}{qarEnd}";
        var reviewPeriod = $"{reviewStart}{reviewEnd}";
        response.QarPeriod = qarPeriod;
        response.ReviewPeriod = reviewPeriod;
        response.CourseDates = null;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns(new ShortlistCookieItem { ShortlistUserId = shortlistUserId });

        locationValidatorMock.Setup(v => v.ValidateAsync(
        It.IsAny<GetCourseLocationQuery>(),
        It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance.ToString() });

        mediator.Setup(x => x.Send(
                It.Is<GetCourseProvidersQuery>(c => c.LarsCode.Equals(request.LarsCode)
                 && c.LocationName.Equals(location)
                 && c.OrderBy.Equals(request.OrderBy)
                 && c.DeliveryModes.SequenceEqual(request.DeliveryModes.Count == 3 ? Array.Empty<ProviderDeliveryMode>() : request.DeliveryModes)
                 && c.EmployerProviderRatings.SequenceEqual(request.EmployerProviderRatings)
                 && c.ApprenticeProviderRatings.SequenceEqual(request.ApprenticeProviderRatings)
                 && c.Qar.SequenceEqual(request.QarRatings)
                 && c.Page.Equals(request.PageNumber)
                 && c.ShortlistUserId.Equals(shortlistUserId)
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var expectedProviders = response.Providers.Select(p => (CoursesProviderViewModel)p).ToList();

        foreach (var provider in expectedProviders)
        {
            provider.Distance = DistanceService.DefaultDistance.ToString();
            provider.LocationName = location;
        }

        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.CourseTitleAndLevel.Should().Be(response.StandardName);
            actualModel.LarsCode.Should().Be(request.LarsCode.ToString());
            actualModel.LocationName.Should().Be(location ?? string.Empty);
            actualModel.Distance.Should().Be(DistanceService.DefaultDistance.ToString());
            actualModel.SelectedDeliveryModes = request.DeliveryModes.Where(dm => dm != ProviderDeliveryMode.Provider)
                .Select(d => d.ToString()).ToList();
            actualModel.SelectedEmployerApprovalRatings.Should()
                .BeEquivalentTo(request.EmployerProviderRatings.Select(r => r.ToString()).ToList());
            actualModel.SelectedApprenticeApprovalRatings.Should()
                .BeEquivalentTo(request.ApprenticeProviderRatings.Select(r => r.ToString()).ToList());
            actualModel.SelectedQarRatings = request.QarRatings.Select(q => q.ToString()).ToList();
            actualModel!.ShowSearchCrumb.Should().BeTrue();
            actualModel.ShowShortListLink.Should().BeTrue();
            actualModel.ShowApprenticeTrainingCourseCrumb.Should().BeTrue();
            actualModel.ShowApprenticeTrainingCoursesCrumb.Should().BeTrue();
            actualModel.QarPeriod.Should().Be(response.QarPeriod);
            actualModel.ReviewPeriod.Should().Be(response.ReviewPeriod);
            actualModel.QarPeriodStartYear.Should().Be($"20{qarStart}");
            actualModel.QarPeriodEndYear.Should().Be($"20{qarEnd}");
            actualModel.ReviewPeriodStartYear.Should().Be($"20{reviewStart}");
            actualModel.ReviewPeriodEndYear.Should().Be($"20{reviewEnd}");
            actualModel.HasCourseFundingStatus.Should().BeFalse();
            actualModel.IsNewCourse.Should().BeFalse();
            actualModel.Providers.Should().BeEquivalentTo(expectedProviders);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndCourseIsClosingSoon_ThenMapsClosingCourseFundingStatus(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        Guid shortlistUserId,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        response.QarPeriod = "2122";
        response.ReviewPeriod = "2223";
        response.CourseDates = new StandardDates
        {
            LastDateStarts = new DateTime(2027, 7, 16, 0, 0, 0, DateTimeKind.Utc),
            EffectiveFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        var providerLastDateStarts = DateTime.UtcNow.Date.AddMonths(4);
        response.Providers[0].LastDateStarts = providerLastDateStarts;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns(new ShortlistCookieItem { ShortlistUserId = shortlistUserId });

        locationValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<GetCourseLocationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = DistanceService.DefaultDistance.ToString() });

        mediator.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var sut = await controller.CourseProviders(request) as ViewResult;
        var actualModel = sut!.Model as CourseProvidersViewModel;

        using (new AssertionScope())
        {
            actualModel.Should().NotBeNull();
            actualModel!.HasCourseFundingStatus.Should().BeTrue();
            actualModel.CourseFundingStatusDescription.Should().Be("Closing to new learners soon");
            actualModel.CourseFundingStatusTagClass.Should().Be("govuk-tag--orange");
            actualModel.CourseFundingStatusMessage.Should().Be("This course is closing on 16 Jul 2027. Government funding will not be available for learners who start after this date.");
            actualModel.IsNewCourse.Should().BeFalse();
            actualModel.Providers[0].ShowProviderCourseAddedDate.Should().BeFalse();
            actualModel.Providers[0].IsLastDateStartsInFuture.Should().BeTrue();
            actualModel.Providers[0].ProviderCourseClosingMessage.Should().Be(
                $"This training provider will stop offering this course on {CourseFundingStatusHelper.FormatDate(providerLastDateStarts)}");
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndCourseIsNew_ThenMapsNewCourseFundingStatusAndCourseAdded(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        Guid shortlistUserId,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        var courseAddedDate = new DateTime(2026, 6, 13, 0, 0, 0, DateTimeKind.Utc);
        response.QarPeriod = "2122";
        response.ReviewPeriod = "2223";
        response.CourseDates = new StandardDates
        {
            LastDateStarts = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc),
            EffectiveFrom = new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc)
        };
        response.Providers[0].ProviderCourseAddedDate = courseAddedDate;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns(new ShortlistCookieItem { ShortlistUserId = shortlistUserId });

        locationValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<GetCourseLocationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = DistanceService.DefaultDistance.ToString() });

        mediator.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var sut = await controller.CourseProviders(request) as ViewResult;
        var actualModel = sut!.Model as CourseProvidersViewModel;

        using (new AssertionScope())
        {
            actualModel.Should().NotBeNull();
            actualModel!.HasCourseFundingStatus.Should().BeTrue();
            actualModel.CourseFundingStatusDescription.Should().Be("New course");
            actualModel.CourseFundingStatusTagClass.Should().Be("govuk-tag--yellow");
            actualModel.CourseFundingStatusMessage.Should().Be("This is a new course. It isn't accepting learners yet. Contact a training provider to find out when it may start.");
            actualModel.IsNewCourse.Should().BeTrue();
            actualModel.Providers[0].ShowProviderCourseAddedDate.Should().BeTrue();
            actualModel.Providers[0].ProviderCourseAddedDateDisplay.Should().Be("13 Jun 2026");
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndCourseIsClosed_ThenMapsClosedCourseFundingStatus(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        Guid shortlistUserId,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        response.QarPeriod = "2122";
        response.ReviewPeriod = "2223";
        response.CourseDates = new StandardDates
        {
            LastDateStarts = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
            EffectiveFrom = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns(new ShortlistCookieItem { ShortlistUserId = shortlistUserId });

        locationValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<GetCourseLocationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = DistanceService.DefaultDistance.ToString() });

        mediator.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var sut = await controller.CourseProviders(request) as ViewResult;
        var actualModel = sut!.Model as CourseProvidersViewModel;

        using (new AssertionScope())
        {
            actualModel.Should().NotBeNull();
            actualModel!.HasCourseFundingStatus.Should().BeTrue();
            actualModel.CourseFundingStatusDescription.Should().Be("Closed to new learners");
            actualModel.CourseFundingStatusTagClass.Should().Be("govuk-tag--grey");
            actualModel.CourseFundingStatusMessage.Should().Be("This course stopped accepting new learners on 30 Jun 2026. Apprentices who are already on this course can still change training providers.");
            actualModel.IsNewCourse.Should().BeFalse();
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndNoShortlistCookie_ThenSendsQueryWithNullShortlistUserId(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.Is<GetCourseProvidersQuery>(c => c.LarsCode.Equals(request.LarsCode)
                 && c.LocationName.Equals(location)
                 && c.OrderBy.Equals(request.OrderBy)
                 && c.DeliveryModes.SequenceEqual(request.DeliveryModes.Count == 3 ? Array.Empty<ProviderDeliveryMode>() : request.DeliveryModes)
                 && c.EmployerProviderRatings.SequenceEqual(request.EmployerProviderRatings)
                 && c.ApprenticeProviderRatings.SequenceEqual(request.ApprenticeProviderRatings)
                 && c.Qar.SequenceEqual(request.QarRatings)
                 && c.Page.Equals(request.PageNumber)
                 && c.ShortlistUserId.Equals(null)
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndLarsCodeIsInvalid_ThenReturnsNotFound(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new("LarsCode", GetCourseQueryValidator.LarsCodeErrorMessage )
            }
        });
        //Act
        var sut = await controller.CourseProviders(request);

        //Assert
        sut.Should().BeOfType<NotFoundResult>();

        mediator.Verify(m => m.Send(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndNoCourseProvidersExist_ThenReturnsNotFound(
        CourseProvidersFiltersRequestModel request,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = null });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseProvidersDetails)null);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        //Act
        var sut = await controller.CourseProviders(request);

        //Assert
        sut.Should().BeOfType<NotFoundResult>();

        mediator.Verify(m => m.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndDistanceIsNull_ThenDefaultsToTenMiles(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = null });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.Distance.Should().Be(DistanceService.DefaultDistance.ToString());
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndLocationEnteredForFirstTime_ThenOrdersByDistance(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        controller.TempData = tempDataMock.Object;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.OrderBy.Should().Be(ProviderOrderBy.Distance);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndLocationPreviouslyEntered_ThenOrdersByUserChoice(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.OrderBy.Should().Be(request.OrderBy);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndNoLocation_ThenOrdersByAchievementRate(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = string.Empty, Distance = null });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.OrderBy.Should().Be(ProviderOrderBy.AchievementRate);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndDistanceExistsInLocationCookie_ThenUsesProvidedDistance(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        string distance = DistanceService.DefaultDistance.ToString();

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.Distance.Should().Be(distance);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndLocationIsCookie_ThenDefaultsDistanceAndOrderBy(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = null, Distance = null });

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.Distance.Should().Be(DistanceService.DefaultDistance.ToString());
            actualModel.OrderBy.Should().Be(ProviderOrderBy.AchievementRate);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndReviewPeriodProvided_ThenSetsReviewPeriodDetails(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string location,
        string distance,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        controller.AddUrlHelperMock();
        controller.TempData = tempDataMock.Object;

        var startReviewPeriod = "22";
        var endReviewPeriod = "23";
        response.ReviewPeriod = $"{startReviewPeriod}{endReviewPeriod}";

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.ReviewPeriodStartYear.Should().Be($"20{startReviewPeriod}");
            actualModel.ReviewPeriodEndYear.Should().Be($"20{endReviewPeriod}");
            actualModel.ProviderReviewsHeading.Should().Be($"Provider reviews in {actualModel.ReviewPeriodStartYear} to {actualModel.ReviewPeriodEndYear}");
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndQarPeriodProvided_ThenSetsQarPeriodDetails(
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string location,
        string distance,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        controller.AddUrlHelperMock();
        controller.TempData = tempDataMock.Object;

        var startQarPeriod = "22";
        var endQarPeriod = "23";
        response.QarPeriod = $"{startQarPeriod}{endQarPeriod}";

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.QarPeriodStartYear.Should().Be($"20{startQarPeriod}");
            actualModel.QarPeriodEndYear.Should().Be($"20{endQarPeriod}");
            actualModel.CourseAchievementRateHeading.Should().Be($"Course achievement rate in {actualModel.QarPeriodStartYear} to {actualModel.QarPeriodEndYear}");
        }
    }

    [Test]
    [MoqInlineAutoData(0, "No results")]
    [MoqInlineAutoData(1, "1 result")]
    [MoqInlineAutoData(2, "2 results")]
    [MoqInlineAutoData(-1, "No results")]
    public async Task WhenGettingCourseProviders_AndVariousTotalCountsProvided_ThenSetsTotalMessageCorrectly(
        int totalCount,
        string expectedMessage,
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        controller.AddUrlHelperMock();
        controller.TempData = tempDataMock.Object;

        response.TotalCount = totalCount;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();

            // Derive expected message from the returned model to match current production formatting
            var totalToUse = actualModel!.TotalCount <= 0 ? "No" : actualModel.TotalCount.ToString();
            var expectedMessageFromModel = $"{totalToUse} result{(actualModel.TotalCount == 1 ? string.Empty : "s")}";
            if (!string.IsNullOrEmpty(actualModel.LocationName) && !string.IsNullOrEmpty(actualModel.Distance) &&
                actualModel.Distance != DistanceService.AcrossEnglandFilterValue)
            {
                expectedMessageFromModel = $"{expectedMessageFromModel} within {actualModel.Distance} miles";
            }

            actualModel!.TotalMessage.Should().Be(expectedMessageFromModel);
        }
    }

    [Test]
    [MoqInlineAutoData(1, "Coventry", "10", "1 result within 10 miles")]
    [MoqInlineAutoData(2, "Coventry", "20", "2 results within 20 miles")]
    [MoqInlineAutoData(2, "Coventry", DistanceService.AcrossEnglandFilterValue, "2 results")]
    [MoqInlineAutoData(2, "Coventry", "", "2 results within 10 miles")]
    public async Task WhenGettingCourseProviders_AndLocationAndDistanceProvided_ThenSetsTotalMessageWithDistanceDetails(
        int totalCount,
        string location,
        string distance,
        string expectedMessage,
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        controller.AddUrlHelperMock();
        controller.TempData = tempDataMock.Object;

        response.TotalCount = totalCount;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);
        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        locationValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.TotalMessage.Should().Be(expectedMessage);
        }
    }

    [Test]
    [MoqInlineAutoData(ProviderOrderBy.Distance, true, false, false, false)]
    [MoqInlineAutoData(ProviderOrderBy.AchievementRate, false, true, false, false)]
    [MoqInlineAutoData(ProviderOrderBy.EmployerProviderRating, false, false, true, false)]
    [MoqInlineAutoData(ProviderOrderBy.ApprenticeProviderRating, false, false, false, true)]
    public async Task WhenGettingCourseProviders_AndVariousOrderByOptionsProvided_ThenSetsProviderOrderDropdownCorrectly(
        ProviderOrderBy orderBy,
        bool distanceSelected,
        bool achievementRateSelected,
        bool employerProviderRatingSelected,
        bool apprenticeProviderRatingSelected,
        CourseProvidersFiltersRequestModel request,
        CourseProvidersDetails response,
        string location,
        string distance,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        controller.AddUrlHelperMock();
        controller.TempData = tempDataMock.Object;

        request.OrderBy = orderBy;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        locationValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        var expectedProviderOrderDropdown = new List<ProviderOrderByOptionViewModel>
        {
            new()
            {
                Description = ProviderOrderBy.Distance.GetDescription(),
                ProviderOrderBy = ProviderOrderBy.Distance,
                Selected = distanceSelected
            },
            new()
            {
                Description = ProviderOrderBy.AchievementRate.GetDescription(),
                ProviderOrderBy = ProviderOrderBy.AchievementRate,
                Selected = achievementRateSelected
            },
            new()
            {
                Description = ProviderOrderBy.EmployerProviderRating.GetDescription(),
                ProviderOrderBy = ProviderOrderBy.EmployerProviderRating,
                Selected = employerProviderRatingSelected
            },
            new()
            {
                Description = ProviderOrderBy.ApprenticeProviderRating.GetDescription(),
                ProviderOrderBy = ProviderOrderBy.ApprenticeProviderRating,
                Selected = apprenticeProviderRatingSelected
            }
        };

        //Act

        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.ProviderOrderOptions.Should().BeEquivalentTo(expectedProviderOrderDropdown);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndShortlistCountInSession_ThenPopulatesShortlistCount(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController sut,
        int shortlistCount,
        CourseProvidersDetails mediatorResult)
    {
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);
        shortlistCookieServiceMock.Setup(s => s.Get(Constants.ShortlistCookieName)).Returns(new ShortlistCookieItem { ShortlistUserId = Guid.NewGuid() });
        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem());

        locationValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        sessionServiceMock.Setup(s => s.Get<ShortlistsCount>(SessionKeys.ShortlistCount)).Returns(new ShortlistsCount { Count = shortlistCount });
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel() { LarsCode = "1" }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ShortlistCount.Should().Be(shortlistCount);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndLocationProvided_ThenIncludesDistanceInOrderByOptions(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Greedy] CourseProvidersController sut,
        CourseProvidersDetails mediatorResult)
    {
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);
        validatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = "CV1 Coventry", Distance = "10" });

        locationValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel() { LarsCode = "1" }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ProviderOrderOptions.Should().Contain(c => c.ProviderOrderBy == ProviderOrderBy.Distance);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndLocationIsEmpty_ThenExcludesDistanceFromOrderByOptions(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController sut,
        CourseProvidersDetails mediatorResult)
    {
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = string.Empty, Distance = null });

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel() { LarsCode = "1" }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ProviderOrderOptions.Should().NotContain(c => c.ProviderOrderBy == ProviderOrderBy.Distance);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndOrderByIsDistanceAndLocationMissing_ThenDefaultsToAchievementRate(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
        [Greedy] CourseProvidersController sut,
        CourseProvidersDetails mediatorResult)
    {
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = string.Empty, Distance = null });

        locationValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        validatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel() { LarsCode = "1", OrderBy = ProviderOrderBy.Distance }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().OrderBy.Should().Be(ProviderOrderBy.AchievementRate);
    }

    [MoqInlineAutoData(ProviderOrderBy.Distance)]
    [MoqInlineAutoData(ProviderOrderBy.AchievementRate)]
    [MoqInlineAutoData(ProviderOrderBy.EmployerProviderRating)]
    [MoqInlineAutoData(ProviderOrderBy.ApprenticeProviderRating)]
    public async Task WhenGettingCourseProviders_AndLocationProvided_ThenMaintainsSelectedOrderBy(
        ProviderOrderBy expectedOrderBy,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController sut,
        CourseProvidersDetails mediatorResult)
    {
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel() { LarsCode = "1", OrderBy = expectedOrderBy }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().OrderBy.Should().Be(expectedOrderBy);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviders_AndShortlistCountMissingFromSession_ThenDefaultsToZero(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
    [Frozen] Mock<ITempDataDictionary> tempDataMock,
    [Greedy] CourseProvidersController sut,
    CourseProvidersDetails mediatorResult)
    {
        // Arrange
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        mediatorMock
            .Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResult);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        sessionServiceMock
            .Setup(s => s.Get<ShortlistsCount>(SessionKeys.ShortlistCount))
            .Returns((ShortlistsCount)null);

        // Act
        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel { LarsCode = "1" }) as ViewResult;

        // Assert
        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ShortlistCount.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task WhenClearLocationQueryParameterPresent_ThenDeletesLocationCookieAndReturnsInvalidViewModel(
           [Frozen] Mock<IValidator<GetCourseQuery>> courseIdValidator,
           [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
           [Greedy] CourseProvidersController sut)
    {
        // Arrange
        var request = new CourseProvidersFiltersSubmitModel { LarsCode = "123" };

        courseIdValidator.Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var locationCookieItem = new LocationCookieItem { LocationName = "M1 1AA", Distance = "10" };
        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName)).Returns(locationCookieItem);

        locationCookieService.Setup(x => x.Delete(It.IsAny<string>()));
        locationCookieService.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<LocationCookieItem>()));

        // Act
        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel { LarsCode = "123" }, true) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as CourseProvidersViewModel;
        model.Should().NotBeNull();
        model.Providers.Should().BeEmpty();
        locationCookieService.Verify(x => x.Delete(It.IsAny<string>()), Times.Once);
        locationCookieService.Verify(x => x.Update(It.IsAny<string>(), It.Is<LocationCookieItem>(i => i == null || i.LocationName == string.Empty)), Times.Never);
    }


    [Test, MoqAutoData]
    public async Task WhenGettingCourseProviderDetails_AndClearLocation_ThenDeletesCookieLocationDistanceStillPresent(
       string larsCode,
       int ukprn,
       [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> providerValidatorMock,
       [Frozen] Mock<IValidator<GetCourseQuery>> larsCodeValidatorMock,
       [Frozen] Mock<IValidator<GetCourseLocationQuery>> locationValidatorMock,
       [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
       [Greedy] CourseProvidersController controller)
    {
        var clearLocation = true;

        providerValidatorMock
           .Setup(x => x.ValidateAsync(It.IsAny<GetCourseProviderDetailsQuery>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new ValidationResult());

        larsCodeValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        locationValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName)).Returns(new LocationCookieItem { LocationName = "Some location", Distance = "20" });

        var result = await controller.CourseProviderDetails(larsCode, ukprn, clearLocation) as ViewResult;

        // Assert
        locationCookieService.Verify(x => x.Update(Constants.LocationCookieName, It.Is<LocationCookieItem>(i => i.LocationName == string.Empty && i.Distance == "20")), Times.Once);

        var model = result!.Model as CourseProviderViewModel;
        model.Should().NotBeNull();
        model.LocationName.Should().BeEmpty();
        model.Distance.Should().Be("20");
    }
}
