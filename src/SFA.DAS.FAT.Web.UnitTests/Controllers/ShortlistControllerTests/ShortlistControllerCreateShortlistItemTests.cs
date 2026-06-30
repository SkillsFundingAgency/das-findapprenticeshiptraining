using System.Text;
using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests;

public class ShortlistControllerCreateShortlistItemTests
{
    [Test, MoqAutoData]
    public async Task CookieExists_AddsToShortlistForUser(
        Guid expectedId,
        CreateShortlistItemRequest request,
        ShortlistCookieItem shortlistCookie,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] ShortlistController controller)
    {
        //Arrange
        mockMediator.Setup(x => x.Send(It.Is<CreateShortlistItemForUserCommand>(c =>
            c.ShortlistUserId == shortlistCookie.ShortlistUserId
            && c.Ukprn == request.Ukprn
            && c.LarsCode == request.LarsCode
        ), It.IsAny<CancellationToken>())).ReturnsAsync(expectedId);
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookie);
        request.RouteName = string.Empty;

        //Act
        var actual = await controller.CreateShortlistItem(request) as AcceptedResult;

        //Assert
        actual.Should().NotBeNull();
        Guid.Parse(actual.Value.ToString()).Should().Be(expectedId);
        mockShortlistCookieService.Verify(x => x.Update(Constants.ShortlistCookieName, shortlistCookie, 30), Times.Once);

    }

    [Test, MoqAutoData]
    public async Task CookieDoesNotExist_CreatesAndUsesNewCookieForShortlist(
        CreateShortlistItemRequest request,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] ShortlistController controller)
    {
        //Arrange
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns((ShortlistCookieItem)null);
        request.RouteName = string.Empty;

        //Act
        var actual = await controller.CreateShortlistItem(request) as AcceptedResult;

        //Assert
        actual.Should().NotBeNull();
        mockMediator.Verify(x => x.Send(It.Is<CreateShortlistItemForUserCommand>(c =>
              c.Ukprn == request.Ukprn
              && c.LarsCode.Equals(request.LarsCode)
        ), It.IsAny<CancellationToken>()), Times.Once);
        mockShortlistCookieService.Verify(x =>
            x.Update(
                Constants.ShortlistCookieName,
                It.Is<ShortlistCookieItem>(c => c.ShortlistUserId != Guid.Empty),
                30), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task RouteNameProvided_RedirectsToRouteWithIdAndUkprn(CreateShortlistItemRequest request,
        ShortlistCookieItem shortlistCookie,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] ShortlistController controller)
    {
        //Arrange
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookie);
        request.RouteName = RouteNames.CourseProviders;

        //Act
        var actual = await controller.CreateShortlistItem(request) as RedirectToRouteResult;

        //Assert
        actual.Should().NotBeNull();
        actual.RouteName.Should().Be(request.RouteName);
        actual.RouteValues.Should().ContainKey("id");
        actual.RouteValues["id"].Should().Be(request.LarsCode);
        actual.RouteValues.Should().ContainKey("ukprn");
        actual.RouteValues["ukprn"].Should().Be(request.Ukprn);
    }

    [Test, MoqAutoData]
    public async Task ProviderNameProvided_EncodesUsingProtector(
        CreateShortlistItemRequest request,
        ShortlistCookieItem shortlistCookie,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<IDataProtector> protector,
        [Frozen] Mock<IDataProtectionProvider> provider,
        [Greedy] ShortlistController controller)
    {
        //Arrange
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookie);
        request.RouteName = RouteNames.CourseProviders;

        //Act
        await controller.CreateShortlistItem(request);

        //Assert
        protector.Verify(c => c.Protect(It.Is<byte[]>(
            x => x[0].Equals(Encoding.UTF8.GetBytes($"{request.ProviderName}")[0]))), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task LocationCookieExists_SendsLocationNameInCreateCommand(
        CreateShortlistItemRequest request,
        ShortlistCookieItem shortlistCookie,
        string locationName,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> mockLocationCookieService,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] ShortlistController controller)
    {
        // Arrange
        request.RouteName = string.Empty;
        mockShortlistCookieService
            .Setup(service => service.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookie);
        mockLocationCookieService
            .Setup(service => service.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = locationName });

        // Act
        await controller.CreateShortlistItem(request);

        // Assert
        mockMediator.Verify(x => x.Send(It.Is<CreateShortlistItemForUserCommand>(c =>
            c.ShortlistUserId == shortlistCookie.ShortlistUserId &&
            c.Ukprn == request.Ukprn &&
            c.LarsCode == request.LarsCode &&
            c.LocationName == locationName),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
