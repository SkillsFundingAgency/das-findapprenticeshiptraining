using AutoFixture.NUnit3;
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

public class WhenGettingCourseProviders
{
    [Test, MoqAutoData]
    public async Task CourseProviders_WithValidRequest_ReturnsViewWithCorrectData(
        CourseProvidersRequest request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        Guid shortlistUserId,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
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

        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);
        controller.TempData = tempDataMock.Object;

        request.Location = location;

        request.Distance = "123";

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

        mediator.Setup(x => x.Send(
                It.Is<GetCourseProvidersQuery>(c => c.LarsCode.Equals(request.LarsCode)
                 && c.Location.Equals(request.Location)
                 && c.OrderBy.Equals(request.OrderBy)
                 && c.DeliveryModes.SequenceEqual(request.DeliveryModes)
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
            provider.Distance = DistanceService.TEN_MILES.ToString();
            provider.Location = request.Location;
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
            actualModel.Location.Should().Be(request.Location ?? string.Empty);
            actualModel.Distance.Should().Be(DistanceService.TEN_MILES.ToString());
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
        CourseProvidersRequest request,
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

        request.Location = location;
        request.Distance = "123";
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.Is<GetCourseProvidersQuery>(c => c.LarsCode.Equals(request.LarsCode)
                 && c.Location.Equals(request.Location)
                 && c.OrderBy.Equals(request.OrderBy)
                 && c.DeliveryModes.SequenceEqual(request.DeliveryModes)
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
    public async Task CourseProviders_WhenCourseIdIsInvalid_ReturnsNotFound(
        CourseProvidersRequest request,
        CourseProvidersDetails response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
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

        request.Location = location;
        request.Distance = null;
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
                new("CourseId", GetCourseQueryValidator.CourseIdErrorMessage )
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
        CourseProvidersRequest request,
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

        request.Location = location;
        request.Distance = null;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

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
        CourseProvidersRequest request,
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

        request.Location = location;
        request.Distance = null;
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
            actualModel!.Distance.Should().Be(DistanceService.TEN_MILES.ToString());
        }
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenLocationEnteredForFirstTime_OrdersByDistance(
        CourseProvidersRequest request,
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

        tempDataMock.SetupGet(t => t[CourseProvidersController.LocationTempDataKey]).Returns((string)null);
        controller.TempData = tempDataMock.Object;

        request.Location = location;
        request.Distance = null;
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
        CourseProvidersRequest request,
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

        tempDataMock.SetupGet(t => t[CourseProvidersController.LocationTempDataKey]).Returns("entered location");
        controller.TempData = tempDataMock.Object;

        request.Location = location;
        request.Distance = null;
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
            tempDataMock.VerifyGet(t => t[CourseProvidersController.LocationTempDataKey], Times.Once);
        }
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenNoLocation_OrdersByAchievementRate(
        CourseProvidersRequest request,
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

        tempDataMock.SetupGet(t => t[CourseProvidersController.LocationTempDataKey]).Returns((string)null);
        controller.TempData = tempDataMock.Object;

        request.Location = null;
        request.Distance = null;
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
            actualModel!.OrderBy.Should().Be(ProviderOrderBy.AchievementRate);
        }
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenDistanceIsProvided_UsesProvidedDistance(
        CourseProvidersRequest request,
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

        request.Location = location;
        request.Distance = "5";
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
            actualModel!.Distance.Should().Be(request.Distance.ToString());
        }
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenLocationIsNotSet_DefaultsDistanceAndOrderBy(
        CourseProvidersRequest request,
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

        request.Location = null;
        request.Distance = null;
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
            actualModel!.Distance.Should().Be(DistanceService.TEN_MILES.ToString());
            actualModel.OrderBy.Should().Be(ProviderOrderBy.AchievementRate);
        }
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenReviewPeriodProvided_SetsReviewPeriodDetails(
        CourseProvidersRequest request,
        CourseProvidersDetails response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
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
        CourseProvidersRequest request,
        CourseProvidersDetails response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
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
        CourseProvidersRequest request,
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
        request.Location = null;
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
            actualModel!.TotalMessage.Should().Be(expectedMessage);
        }
    }

    [Test]
    [MoqInlineAutoData(1, "Coventry", "10", "1 result within 10 miles")]
    [MoqInlineAutoData(2, "Coventry", "20", "2 results within 20 miles")]
    [MoqInlineAutoData(2, "Coventry", DistanceService.ACROSS_ENGLAND_FILTER_VALUE, "2 results")]
    [MoqInlineAutoData(2, "Coventry", "", "2 results within 10 miles")]
    public async Task CourseProviders_WithLocationAndDistance_SetsTotalMessageWithDistanceDetails(
        int totalCount,
        string location,
        string distance,
        string expectedMessage,
        CourseProvidersRequest request,
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
        request.Location = location;
        request.Distance = distance;
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
        CourseProvidersRequest request,
        CourseProvidersDetails response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController controller)
    {
        controller.AddUrlHelperMock();
        controller.TempData = tempDataMock.Object;

        request.OrderBy = orderBy;

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
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] CourseProvidersController sut,
        int shortlistCount,
        CourseProvidersDetails mediatorResult)
    {
        sut.AddUrlHelperMock();
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);
        shortlistCookieServiceMock.Setup(s => s.Get(Constants.ShortlistCookieName)).Returns(new ShortlistCookieItem { ShortlistUserId = Guid.NewGuid() });
        sessionServiceMock.Setup(s => s.Get<ShortlistsCount>(SessionKeys.ShortlistCount)).Returns(new ShortlistsCount { Count = shortlistCount });
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());
        var result = await sut.CourseProviders(new CourseProvidersRequest() { LarsCode = "1" }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ShortlistCount.Should().Be(shortlistCount);
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenLocationProvided_IncludesDistanceInOrderByOptions(
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

        var result = await sut.CourseProviders(new CourseProvidersRequest() { LarsCode = "1", Location = "CV1 Coventry" }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ProviderOrderOptions.Should().Contain(c => c.ProviderOrderBy == ProviderOrderBy.Distance);
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenLocationIsEmpty_ExcludesDistanceFromOrderByOptions(
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

        var result = await sut.CourseProviders(new CourseProvidersRequest() { LarsCode = "1", Location = string.Empty }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().ProviderOrderOptions.Should().NotContain(c => c.ProviderOrderBy == ProviderOrderBy.Distance);
    }

    [Test, MoqAutoData]
    public async Task CourseProviders_WhenOrderByIsDistanceAndLocationMissing_DefaultsToAchievementRate(
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
        var result = await sut.CourseProviders(new CourseProvidersRequest() { LarsCode = "1", Location = string.Empty, OrderBy = ProviderOrderBy.Distance }) as ViewResult;

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
        var result = await sut.CourseProviders(new CourseProvidersRequest() { LarsCode = "1", Location = "CV1 Coventry", OrderBy = expectedOrderBy }) as ViewResult;

        result.As<ViewResult>().Model.As<CourseProvidersViewModel>().OrderBy.Should().Be(expectedOrderBy);
    }
}
