using System.Text;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests;

public class WhenAddingAShortlistItemForUser
{
    [Test, MoqAutoData]
    public async Task And_Cookie_Exists_Then_Adds_To_Shortlist_For_User(
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
            && c.LocationName == request.LocationName
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
    public async Task And_The_Cookie_Does_Not_Exist_Then_A_New_Cookie_Is_Created_And_Used_For_Shortlist(
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
              c.LocationName == request.LocationName
              && c.Ukprn == request.Ukprn
              && c.LarsCode.Equals(request.LarsCode)
        ), It.IsAny<CancellationToken>()), Times.Once);
        mockShortlistCookieService.Verify(x =>
            x.Update(
                Constants.ShortlistCookieName,
                It.Is<ShortlistCookieItem>(c => c.ShortlistUserId != Guid.Empty),
                30), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task And_If_There_Is_A_Route_Name_Then_It_Is_Redirected(CreateShortlistItemRequest request,
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
        actual.RouteValues.Should().ContainKey("providerId");
        actual.RouteValues["providerId"].Should().Be(request.Ukprn);
    }

    [Test, MoqAutoData]
    public async Task And_If_ProviderName_Is_In_The_Request_Is_Encoded_Using_The_Protector(
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
}
