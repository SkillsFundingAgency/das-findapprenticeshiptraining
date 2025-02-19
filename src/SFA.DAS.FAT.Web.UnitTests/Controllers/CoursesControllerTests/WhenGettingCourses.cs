using AutoFixture.NUnit3;
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
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests;

public class WhenGettingCourses
{
    [Test]
    [MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_View_Shown(
        GetCoursesRequest request,
        GetCoursesQueryResult response,
        ShortlistCookieItem cookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CoursesController controller
    )
    {
        request.Distance = "1000";

        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString())
            .AddUrlForRoute(RouteNames.ShortList, Guid.NewGuid().ToString());

        mediator.Setup(x =>
            x.Send(
                It.Is<GetCoursesQuery>(c =>
                    c.Keyword.Equals(request.Keyword) &&
                    c.Location.Equals(request.Location) &&
                    c.Distance.Equals(request.Distance) &&
                    c.Levels.SequenceEqual(request.Levels) &&
                    c.Routes.SequenceEqual(request.Categories) &&
                    c.ShortlistUserId.Equals(cookieItem.ShortlistUserId)
                ), 
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(response);

        shortlistCookieService.Setup(x =>
            x.Get(Constants.ShortlistCookieName)
        )
        .Returns(cookieItem);

        var _sut = await controller.Courses(request);
        var actualResult = _sut as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.Not.Null);
            Assert.That(actualResult, Is.Not.Null);
        });
    }

    [Test, MoqAutoData]
    public async Task Should_Add_Keyword_Categories_Levels_And_Location_To_Query_And_Return_View(
        GetCoursesRequest request,
        GetCoursesQueryResult response,
        Guid shortlistUrl,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        [Greedy] CoursesController controller
    )
    {
        controller.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString())
            .AddUrlForRoute(RouteNames.ShortList, shortlistUrl.ToString());

        mediator.Setup(x =>
            x.Send(
                It.Is<GetCoursesQuery>(c =>
                    c.Keyword.Equals(request.Keyword) &&
                    c.Location.Equals(request.Location) &&
                    c.Distance.Equals(1000) &&
                    c.Levels.SequenceEqual(request.Levels) &&
                    c.Routes.SequenceEqual(request.Categories)
                ),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(response);

        shortlistCookieService.Setup(x => 
            x.Get(Constants.ShortlistCookieName)
        )
        .Returns((ShortlistCookieItem)null);

        var _sut = await controller.Courses(request);

        var expectedStandards = response.Standards.Select(s => (StandardViewModel)s).ToList();
        var expectedRoutes = response.Routes.Select(r => new RouteViewModel(r, request.Categories)).ToList();
        var expectedLevels = response.Levels.Select(l => new LevelViewModel(l, request.Levels)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(_sut, Is.Not.Null);
            var result = _sut as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result!.Model as CoursesViewModel;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.TotalFiltered, Is.EqualTo(response.TotalCount));

            model.Standards.Should().BeEquivalentTo(expectedStandards);
            model.Routes.Should().BeEquivalentTo(expectedRoutes);
            model.Levels.Should().BeEquivalentTo(expectedLevels);

            Assert.That(model.Keyword, Is.EqualTo(request.Keyword));
            Assert.That(model.SelectedLevels, Is.EquivalentTo(request.Levels));
            Assert.That(model.SelectedRoutes, Is.EquivalentTo(request.Categories));
            Assert.That(model.ShowShortListLink, Is.True);
            Assert.That(model.ShowSearchCrumb, Is.True);
        });
    }
}
