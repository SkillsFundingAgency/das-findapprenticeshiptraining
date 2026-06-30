using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItem;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests;

public class ShortlistControllerDeletingShortlistItemTests
{
    [Test, MoqAutoData]
    public async Task CookieExists_DeletesShortlistItemForUser(
        DeleteShortlistItemRequest request,
        ShortlistCookieItem shortlistCookie,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] ShortlistController controller)
    {
        //Arrange
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookie);
        request.RouteName = string.Empty;

        //Act
        var actual = await controller.DeleteShortlistItemForUser(request) as AcceptedResult;

        //Assert
        actual.Should().NotBeNull();
        mockMediator.Verify(x => x.Send(It.Is<DeleteShortlistItemCommand>(c => c.Id.Equals(request.ShortlistId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task RouteNameProvided_RedirectsToRouteWithLarsCodeAndUkprn(
        Guid id,
        DeleteShortlistItemRequest request,
        ShortlistCookieItem shortlistCookie,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IDataProtector> protector,
        [Greedy] ShortlistController sut)
    {
        //Arrange
        sut.TempData = tempDataMock.Object;
        request.ProviderName = string.Empty;
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookie);
        request.RouteName = RouteNames.CourseProviders;

        //Act
        var actual = await sut.DeleteShortlistItemForUser(request) as RedirectToRouteResult;

        //Assert
        actual.Should().NotBeNull();
        actual.RouteName.Should().Be(RouteNames.CourseProviders);
        actual.RouteValues.Should().ContainKey("larsCode");
        actual.RouteValues["larsCode"].Should().Be(request.LarsCode);
        actual.RouteValues.Should().ContainKey("ukprn");
        actual.RouteValues["ukprn"].Should().Be(request.Ukprn);
        protector.Verify(c => c.Protect(It.IsAny<byte[]>()), Times.Never);
    }
}
