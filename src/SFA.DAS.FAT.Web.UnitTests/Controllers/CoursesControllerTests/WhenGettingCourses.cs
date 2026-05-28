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
        GetCoursesViewModel request,
        GetCoursesQueryResult queryResult,
        ShortlistCookieItem cookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
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
        GetCoursesViewModel request,
        GetCoursesQueryResult queryResult,
        Guid shortlistUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
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
        GetCoursesViewModel request,
        GetCoursesQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
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
        GetCoursesViewModel request,
        GetCoursesQueryResult response,
        [Frozen] Mock<IMediator> mediator,
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
}
