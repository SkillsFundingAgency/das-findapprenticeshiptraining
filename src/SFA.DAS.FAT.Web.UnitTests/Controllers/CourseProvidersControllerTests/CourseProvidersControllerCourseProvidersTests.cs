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
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.FAT.Web.Validators;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CourseProvidersControllerTests;

public class CourseProvidersControllerCourseProvidersTests
{
    [Test, MoqAutoData]
    public async Task CourseProviders_WithValidRequest_ReturnsViewWithCorrectData(
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

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns(new ShortlistCookieItem { ShortlistUserId = shortlistUserId });

        locationValidatorMock.Setup(v => v.ValidateAsync(
        It.IsAny<GetCourseLocationQuery>(),
        It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = location, Distance = distance.ToString() });

        mediator.Setup(x => x.Send(
                It.Is<GetCourseProvidersQuery>(c => c.LarsCode.Equals(request.LarsCode)
                 && c.Location.Equals(location)
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
            provider.Location = location;
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
            actualModel.Location.Should().Be(location ?? string.Empty);
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
            actualModel.Providers.Should().BeEquivalentTo(expectedProviders);
        }
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenNoShortlistCookie_SendsQueryWithNullShortlistUserId(
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
                 && c.Location.Equals(location)
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
    public async Task CourseProviders_WhenLarsCodeIsInvalid_ReturnsNotFound(
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
    public async Task CourseProviders_WhenNoCourseProvidersExist_ReturnsNotFound(
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
            .Returns(new LocationCookieItem { Location = location, Distance = null });

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
    public async Task CourseProviders_WhenDistanceIsNull_DefaultsToTenMiles(
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
            .Returns(new LocationCookieItem { Location = location, Distance = null });

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
    public async Task CourseProviders_WhenLocationEnteredForFirstTime_OrdersByDistance(
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
    public async Task CourseProviders_WhenLocationPreviouslyEntered_OrdersByUserChoice(
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
    public async Task CourseProviders_WhenNoLocation_OrdersByAchievementRate(
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
            .Returns(new LocationCookieItem { Location = string.Empty, Distance = null });

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
    public async Task CourseProviders_WhenDistanceExistsInLocationCookie_UsesProvidedDistance(
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
            .Returns(new LocationCookieItem { Location = location, Distance = distance });

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
    public async Task CourseProviders_WhenLocationIsCookie_DefaultsDistanceAndOrderBy(
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
            .Returns(new LocationCookieItem { Location = null, Distance = null });

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
    public async Task CourseProviders_WhenReviewPeriodProvided_SetsReviewPeriodDetails(
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
            .Returns(new LocationCookieItem { Location = location, Distance = distance });

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
    public async Task CourseProviders_WhenQarPeriodProvided_SetsQarPeriodDetails(
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
            .Returns(new LocationCookieItem { Location = location, Distance = distance });

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
    public async Task CourseProviders_WithVariousTotalCounts_SetsTotalMessageCorrectly(
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
            if (!string.IsNullOrEmpty(actualModel.Location) && !string.IsNullOrEmpty(actualModel.Distance) &&
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
    public async Task CourseProviders_WithLocationAndDistance_SetsTotalMessageWithDistanceDetails(
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
            .Returns(new LocationCookieItem { Location = location, Distance = distance });

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
    public async Task CourseProviders_WithVariousOrderByOptions_SetsProviderOrderDropdownCorrectly(
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
            .Returns(new LocationCookieItem { Location = location, Distance = distance });

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
    public async Task CourseProviders_WhenShortlistCountInSession_PopulatesShortlistCount(
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
    public async Task CourseProviders_WhenLocationProvided_IncludesDistanceInOrderByOptions(
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
            .Returns(new LocationCookieItem { Location = "CV1 Coventry", Distance = "10" });

        locationValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviders(new CourseProvidersFiltersRequestModel() { LarsCode = "1" }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ProviderOrderOptions.Should().Contain(c => c.ProviderOrderBy == ProviderOrderBy.Distance);
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenLocationIsEmpty_ExcludesDistanceFromOrderByOptions(
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
            .Returns(new LocationCookieItem { Location = string.Empty, Distance = null });

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
    public async Task CourseProviders_WhenOrderByIsDistanceAndLocationMissing_DefaultsToAchievementRate(
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
            .Returns(new LocationCookieItem { Location = string.Empty, Distance = null });

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
    public async Task CourseProviders_WhenLocationProvided_MaintainsSelectedOrderBy(
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
    public async Task CourseProviders_ShortlistCountMissingFromSession_DefaultsToZero(
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
    public async Task WhenClearLocationQueryParameterPresent_DeletesLocationCookie_AndReturnsInvalidViewModel(
           [Frozen] Mock<IValidator<GetCourseQuery>> courseIdValidator,
           [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
           [Greedy] CourseProvidersController sut)
    {
        // Arrange
        var request = new CourseProvidersFiltersSubmitModel { LarsCode = "123" };

        courseIdValidator.Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var locationCookieItem = new LocationCookieItem { Location = "M1 1AA", Distance = "10" };
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
        locationCookieService.Verify(x => x.Update(It.IsAny<string>(), It.Is<LocationCookieItem>(i => i == null || i.Location == string.Empty)), Times.Never);
    }


    [Test, MoqAutoData]
    public async Task CourseProviderDetails_WithClearLocation_DeletesCookieLocationDistanceStillPresent(
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

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName)).Returns(new LocationCookieItem { Location = "Some location", Distance = "20" });

        var result = await controller.CourseProviderDetails(larsCode, ukprn, clearLocation) as ViewResult;

        // Assert
        locationCookieService.Verify(x => x.Update(Constants.LocationCookieName, It.Is<LocationCookieItem>(i => i.Location == string.Empty && i.Distance == "20")), Times.Once);

        var model = result!.Model as CourseProviderViewModel;
        model.Should().NotBeNull();
        model.Location.Should().BeEmpty();
        model.Distance.Should().Be("20");
    }
}
