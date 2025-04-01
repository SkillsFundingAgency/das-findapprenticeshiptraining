﻿using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CourseProvidersControllerTests;
public class WhenGettingCourseProviders
{
    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_View_Shown(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        Guid shortlistUserId,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortList, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

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
                It.Is<GetCourseProvidersQuery>(c => c.Id.Equals(request.Id)
                 && c.Location.Equals(request.Location)
                 && c.OrderBy.Equals(request.OrderBy)
                 && c.DeliveryModes.SequenceEqual(request.DeliveryModes)
                 && c.EmployerProviderRatings.SequenceEqual(request.EmployerProviderRatings)
                 && c.ApprenticeProviderRatings.SequenceEqual(request.ApprenticeProviderRatings)
                 && c.Qar.SequenceEqual(request.QarRatings)
                 && c.Page.Equals(request.Page)
                 && c.PageSize.Equals(request.PageSize)
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
            actualModel.CourseId.Should().Be(request.Id);
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
    public async Task Then_No_Shortlist_UserId_Is_Added_To_The_Cookie(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortList, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        request.Location = location;
        request.Distance = "123";
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.Is<GetCourseProvidersQuery>(c => c.Id.Equals(request.Id)
                 && c.Location.Equals(request.Location)
                 && c.OrderBy.Equals(request.OrderBy)
                 && c.DeliveryModes.SequenceEqual(request.DeliveryModes)
                 && c.EmployerProviderRatings.SequenceEqual(request.EmployerProviderRatings)
                 && c.ApprenticeProviderRatings.SequenceEqual(request.ApprenticeProviderRatings)
                 && c.Qar.SequenceEqual(request.QarRatings)
                 && c.Page.Equals(request.Page)
                 && c.PageSize.Equals(request.PageSize)
                 && c.ShortlistUserId.Equals(null)
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    public async Task Then_Distance_Defaults_to_10(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortList, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        request.Location = location;
        request.Distance = null;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    public async Task Then_Distance_Is_Valid(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortList, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        request.Location = location;
        request.Distance = "5";
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    public async Task Then_Location_Is_Not_Set(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        string serviceStartUrl,
        string shortlistUrl,
        string courseDetailsUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Arrange
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, serviceStartUrl)
            .AddUrlForRoute(RouteNames.ShortList, shortlistUrl)
            .AddUrlForRoute(RouteNames.CourseDetails, courseDetailsUrl);

        request.Location = null;
        request.Distance = null;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        //Act
        var sut = await controller.CourseProviders(request) as ViewResult;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            var actualModel = sut!.Model as CourseProvidersViewModel;
            actualModel.Should().NotBeNull();
            actualModel!.Distance.Should().Be(string.Empty);
        }
    }

    [Test, MoqAutoData]
    public async Task Then_ReviewPeriod_Details_Are_Set(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        var startReviewPeriod = "22";
        var endReviewPeriod = "23";
        response.ReviewPeriod = $"{startReviewPeriod}{endReviewPeriod}";

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    public async Task Then_QarPeriod_Details_Are_Set(
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        var startQarPeriod = "22";
        var endQarPeriod = "23";
        response.QarPeriod = $"{startQarPeriod}{endQarPeriod}";

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    [MoqInlineAutoData(500, "500 results")]
    [MoqInlineAutoData(-1, "No results")]
    public async Task Then_TotalMessage_Is_Set(
        int totalCount,
        string expectedMessage,
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        response.TotalCount = totalCount;
        request.Location = null;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    public async Task Then_TotalMessage_With_Distance_And_Location_Is_Set(
        int totalCount,
        string location,
        string distance,
        string expectedMessage,
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        response.TotalCount = totalCount;
        request.Location = location;
        request.Distance = distance;
        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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
    public async Task Then_ProviderOrderDropdown_Is_Set(
        ProviderOrderBy orderBy,
        bool distanceSelected,
        bool achievementRateSelected,
        bool employerProviderRatingSelected,
        bool apprenticeProviderRatingSelected,
        CourseProvidersRequest request,
        GetCourseProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CourseProvidersController controller)
    {
        request.OrderBy = orderBy;

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(x => x.Send(
                It.IsAny<GetCourseProvidersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

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

}
