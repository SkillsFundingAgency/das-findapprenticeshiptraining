using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItemForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests
{
    public class WhenDeletingShortlistItemForUser
    {
        [Test, MoqAutoData]
        public async Task And_Cookie_Exists_Then_Deletes_Shortlist_Item_For_User(
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
            mockMediator.Verify(x=>x.Send(It.Is<DeleteShortlistItemForUserCommand>(c=>
                c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)
                && c.Id.Equals(request.ShortlistId)
            ), It.IsAny<CancellationToken>()), Times.Once);
            
        }
        
        [Test, MoqAutoData]
        public async Task And_Cookie_Does_Not_Exists_Then_Deletes_Command_Is_Not_Called(
            Guid id,
            DeleteShortlistItemRequest request,
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
            var actual = await controller.DeleteShortlistItemForUser(request) as AcceptedResult;
            
            //Assert
            actual.Should().NotBeNull();
            mockMediator.Verify(x=>x.Send(It.IsAny<DeleteShortlistItemForUserCommand>(),
                It.IsAny<CancellationToken>()), Times.Never);
            
        }

        [Test, MoqAutoData]
        public async Task And_If_There_Is_A_RouteName_Then_It_Is_Redirected_And_ProviderName_Not_Encoded_If_Empty(
            Guid id,
            DeleteShortlistItemRequest request,
            ShortlistCookieItem shortlistCookie,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Greedy] ShortlistController controller)
        {
            //Arrange
            request.ProviderName = string.Empty;
            mockShortlistCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            request.RouteName = RouteNames.CourseProviders;
            
            //Act
            var actual = await controller.DeleteShortlistItemForUser(request) as RedirectToRouteResult;
            
            //Assert
            actual.Should().NotBeNull();
            actual.RouteName.Should().Be(RouteNames.CourseProviders);
            actual.RouteValues.Should().ContainKey("id");
            actual.RouteValues["id"].Should().Be(request.TrainingCode);
            actual.RouteValues.Should().ContainKey("providerId");
            actual.RouteValues["providerId"].Should().Be(request.Ukprn);
            actual.RouteValues["removed"].Should().Be(string.Empty);
            protector.Verify(c=>c.Protect(It.IsAny<byte[]>()), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task And_If_ProviderName_Is_In_The_Request_Is_Encoded_Using_The_Protector(
            Guid id,
            DeleteShortlistItemRequest request,
            ShortlistCookieItem shortlistCookie,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockShortlistCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] ShortlistController controller)
        {
            //Arrange
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mockShortlistCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            request.RouteName = RouteNames.CourseProviders;
            
            //Act
            await controller.DeleteShortlistItemForUser(request);
            
            //Assert
            protector.Verify(c=>c.Protect(It.Is<byte[]>(
                x=>x[0].Equals(Encoding.UTF8.GetBytes($"{request.ProviderName}")[0]))), Times.Once);  
        }
        
    }
}
