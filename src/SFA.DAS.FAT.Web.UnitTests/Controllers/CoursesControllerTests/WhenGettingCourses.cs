using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests;

public class WhenGettingCourses
{
    [Test]
    [MoqAutoData]
    public async Task Courses_RequestIsValid_QueryIsSentAndViewIsReturned(
        CoursesFiltersRequestModel request,
        GetCoursesQueryResult queryResult,
        ShortlistCookieItem cookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut
    )
    {
        request.Distance = "10";

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString());

        mediator.Setup(x =>
            x.Send(
                It.Is<GetCoursesQuery>(c =>
                    c.Keyword.Equals(request.Keyword) &&
                    c.Location.Equals(request.Location) &&
                    c.Distance.Equals(DistanceService.TenMiles) &&
                    c.Levels.SequenceEqual(request.Levels) &&
                    c.Routes.SequenceEqual(request.Categories) &&
                    c.Page.Equals(request.PageNumber) &&
                    c.ShortlistUserId.Equals(cookieItem.ShortlistUserId)
                ),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = request.Location, Distance = request.Distance });

        queryResult.Standards.ForEach(S => S.Level = queryResult.Levels.First().Code);

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns(cookieItem);

        var actualResponse = await sut.Courses(request);
        var actualResult = actualResponse as ViewResult;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actualResponse, Is.Not.Null);
            Assert.That(actualResult, Is.Not.Null);
        }
    }

    [Test, MoqAutoData]
    public async Task Courses_RequestContainsSearchFilters_ModelContainsMappedResults(
        CoursesFiltersRequestModel request,
        GetCoursesQueryResult queryResult,
        Guid shortlistUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController controller
    )
    {
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString())
            .AddUrlForRoute(RouteNames.ShortLists, shortlistUrl.ToString());

        queryResult.Standards.ForEach(S => S.Level = queryResult.Levels.First().Code);

        mediator.Setup(x =>
            x.Send(
                It.Is<GetCoursesQuery>(c =>
                    c.Keyword.Equals(request.Keyword) &&
                    c.Location.Equals(request.Location) &&
                    c.Distance.Equals(DistanceService.TenMiles) &&
                    c.Levels.SequenceEqual(request.Levels) &&
                    c.Routes.SequenceEqual(request.Categories) &&
                    c.Page.Equals(request.PageNumber)
                ),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns((ShortlistCookieItem)null);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = request.Location, Distance = request.Distance });

        var _sut = await controller.Courses(request);

        var expectedRoutes = queryResult.Routes.Select(r => new RouteViewModel(r, request.Categories)).ToList();
        var expectedLevels = queryResult.Levels.Select(l => new LevelViewModel(l, request.Levels)).ToList();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut, Is.Not.Null);
            var result = _sut as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result!.Model as CoursesViewModel;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.TotalFiltered, Is.EqualTo(queryResult.TotalCount));

            model.Standards.Should().HaveCount(queryResult.Standards.Count);
            model.Routes.Should().BeEquivalentTo(expectedRoutes);
            model.Levels.Should().BeEquivalentTo(expectedLevels);

            Assert.That(model.Keyword, Is.EqualTo(request.Keyword));
            Assert.That(model.SelectedLevels, Is.EquivalentTo(request.Levels));
            Assert.That(model.SelectedRoutes, Is.EquivalentTo(request.Categories));
            Assert.That(model.ShowShortListLink, Is.True);
            Assert.That(model.ShowSearchCrumb, Is.True);
        }
    }

    [Test, MoqAutoData]
    public async Task Courses_ResultContainsStandards_PaginationIsPopulated(
        CoursesFiltersRequestModel request,
        GetCoursesQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController controller
    )
    {
        queryResult.Standards.ForEach(S => S.Level = queryResult.Levels.First().Code);
        controller.AddUrlHelperMock();

        mediator.Setup(x =>
            x.Send(
                It.Is<GetCoursesQuery>(c =>
                    c.Keyword.Equals(request.Keyword) &&
                    c.Location.Equals(request.Location) &&
                    c.Distance.Equals(DistanceService.TenMiles) &&
                    c.Levels.SequenceEqual(request.Levels) &&
                    c.Routes.SequenceEqual(request.Categories)
                ),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = request.Location, Distance = request.Distance });

        var _sut = await controller.Courses(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut, Is.Not.Null);
            var result = _sut as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result!.Model as CoursesViewModel;

            Assert.That(model.Standards, Has.Count.AtLeast(1));
            Assert.That(model.Pagination, Is.Not.Null);
        }
    }

    [Test, MoqAutoData]
    public async Task Courses_ResultContainsNoStandards_PaginationIsNull(
        CoursesFiltersRequestModel request,
        GetCoursesQueryResult response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController controller
    )
    {
        controller.AddUrlHelperMock();

        response.Standards = [];

        mediator.Setup(x =>
            x.Send(
                It.Is<GetCoursesQuery>(c =>
                    c.Keyword.Equals(request.Keyword) &&
                    c.Location.Equals(request.Location) &&
                    c.Distance.Equals(DistanceService.TenMiles) &&
                    c.Levels.SequenceEqual(request.Levels) &&
                    c.Routes.SequenceEqual(request.Categories)
                ),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(response);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = request.Location, Distance = request.Distance });

        var _sut = await controller.Courses(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut, Is.Not.Null);
            var result = _sut as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result!.Model as CoursesViewModel;

            Assert.That(model.Standards, Has.Count.EqualTo(0));
            Assert.That(model.Pagination, Is.Null);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenClearLocationQueryParameterPresent_DeletesLocationCookie_AndQueryUsesNoLocation(
        CoursesFiltersRequestModel request,
        GetCoursesQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CoursesController controller
    )
    {
        // Arrange
        var clearFilter = true;
        controller.AddUrlHelperMock();
        CoursesViewModel model = new CoursesViewModel();
        queryResult.Standards = [];

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = "London", Distance = "10" });

        shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);

        mediator.Setup(m => m.Send(It.IsAny<GetCoursesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        // Act
        var result = await controller.Courses(request, clearFilter) as ViewResult;

        // Assert
        result.Should().NotBeNull();

        locationCookieService.Verify(x => x.Delete(Constants.LocationCookieName), Times.Once);

        mediator.Verify(x => x.Send(It.IsAny<GetCoursesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task CourseDetailsDelete_WithClearLocation_DeletesLocationCookie(
       string larsCode,
       [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
       [Greedy] CoursesController sut
   )
    {
        var clearLocation = true;

        // Act
        var result = await sut.CourseDetails(larsCode, clearLocation) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        locationCookieService.Verify(x => x.Delete(Constants.LocationCookieName), Times.Once);
    }
}
