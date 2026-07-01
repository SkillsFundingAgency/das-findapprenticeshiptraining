using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests;

public class ShortlistControllerOpenShortlistItemTests
{
    [Test, MoqAutoData]
    public async Task WhenShortlistCookieNotFound_RedirectsToCourseProviderDetails_(
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ShortlistController sut,
        int ukprn,
        string larsCode,
        string locationDescription)
    {
        // Arrange
        shortlistCookieServiceMock.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns(() => null);
        var shortlistId = Guid.NewGuid();

        // Act
        var result = await sut.OpenShortlistItem(shortlistId, ukprn, larsCode);

        // Assert
        using (new AssertionScope())
        {
            shortlistCookieServiceMock.Verify(x => x.Get(Constants.ShortlistCookieName), Times.Once);
            mediatorMock.Verify(x => x.Send(It.IsAny<GetShortlistsForUserQuery>(), default), Times.Never);
            var redirect = result.As<RedirectToRouteResult>();
            redirect.RouteName.Should().Be(RouteNames.CourseProviderDetails);
            redirect.RouteValues["LarsCode"].Should().Be(larsCode);
            redirect.RouteValues["Ukprn"].Should().Be(ukprn);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenNoMatchingProviderFound_RedirectsToCourseProviderDetails(
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieServiceMock,
        [Greedy] ShortlistController sut,
        int ukprn,
        string larsCode,
        GetShortlistsForUserResponse response)
    {
        // Arrange
        var cookie = new ShortlistCookieItem { ShortlistUserId = Guid.NewGuid() };
        shortlistCookieServiceMock.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns(cookie);


        mediatorMock.Setup(x => x.Send(It.IsAny<GetShortlistsForUserQuery>(), default)).ReturnsAsync(response);

        var shortlistId = Guid.NewGuid(); // different id so no match

        // Act
        var result = await sut.OpenShortlistItem(shortlistId, ukprn, larsCode);

        // Assert
        using (new AssertionScope())
        {
            mediatorMock.Verify(x => x.Send(It.IsAny<GetShortlistsForUserQuery>(), default), Times.Once);
            locationCookieServiceMock.Verify(x => x.Update(It.IsAny<string>(), It.Is<LocationCookieItem>(a => a.Location == string.Empty)), Times.Once);
            var redirect = result.As<RedirectToRouteResult>();
            redirect.RouteName.Should().Be(RouteNames.CourseProviderDetails);
            redirect.RouteValues["LarsCode"].Should().Be(larsCode);
            redirect.RouteValues["Ukprn"].Should().Be(ukprn);
        }
    }

    [Test, MoqAutoData]
    public async Task WhenMatchingProviderFound_UpdateLocationCookieAndRedirects(
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieServiceMock,
        [Greedy] ShortlistController sut,
        int ukprn,
        string larsCode,
        string locationDescription)
    {
        // Arrange
        var cookie = new ShortlistCookieItem { ShortlistUserId = Guid.NewGuid() };
        shortlistCookieServiceMock.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns(cookie);

        var shortlistId = Guid.NewGuid();

        var response = new GetShortlistsForUserResponse
        {
            Courses = new List<ShortlistCourseModel>
            {
                new ShortlistCourseModel
                {
                    LarsCode = larsCode,
                    Locations = new List<ShortlistLocationModel>
                    {
                        new ShortlistLocationModel
                        {
                            LocationDescription = locationDescription,
                            Providers = new List<ShortlistProviderModel>
                            {
                                new ShortlistProviderModel { ShortlistId = shortlistId }
                            }
                        }
                    }
                }
            }
        };

        mediatorMock.Setup(x => x.Send(It.IsAny<GetShortlistsForUserQuery>(), default)).ReturnsAsync(response);

        // Act
        var result = await sut.OpenShortlistItem(shortlistId, ukprn, larsCode);

        // Assert
        using (new AssertionScope())
        {
            locationCookieServiceMock.Verify(x => x.Update(Constants.LocationCookieName, It.Is<LocationCookieItem>(a => a.Location == locationDescription)), Times.Once);

            var redirect = result.As<RedirectToRouteResult>();
            redirect.RouteName.Should().Be(RouteNames.CourseProviderDetails);
            redirect.RouteValues["larsCode"].Should().Be(larsCode);
            redirect.RouteValues["ukprn"].Should().Be(ukprn);
        }
    }
}
