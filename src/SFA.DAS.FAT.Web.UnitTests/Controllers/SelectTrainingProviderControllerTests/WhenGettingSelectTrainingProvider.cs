#nullable enable
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.SelectTrainingProviderControllerTests;
public class WhenGettingSelectTrainingProvider
{
    public class WhenGettingShortlistForUser
    {
        [Test, MoqAutoData]
        public async Task And_Cookie_Exists_Then_Reads_Cookie_And_Builds_ViewModel(
            ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            Domain.Shortlist.ShortlistForUser shortlistFromService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IShortlistService> mockService,
            [Greedy] SelectTrainingProviderController controller)
        {
            //Arrange
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);

            mockService
                .Setup(service => service.GetShortlistForUser(shortlistCookie.ShortlistUserId))
                .ReturnsAsync(shortlistFromService);

            //Act
            var actual = await controller.Index() as ViewResult;

            //Assert
            actual.Should().NotBeNull();
            var model = actual!.Model as SelectTrainingProviderViewModel;
            model.Should().NotBeNull();
            model!.ShortListItemCount.Should().Be(shortlistFromService.Shortlist.Count());
            mockService.Verify(x => x.GetShortlistForUser(shortlistCookie.ShortlistUserId), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_Cookie_Does_Not_Exist_Then_Builds_ViewModel(
            GetShortlistForUserResult resultFromMediator,
            Domain.Shortlist.ShortlistForUser shortlistFromService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IShortlistService> mockService,
            [Greedy] SelectTrainingProviderController controller)
        {
            //Arrange
            var shortlistCookie = (ShortlistCookieItem)null!;

            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie!);

            //Act
            var actual = await controller.Index() as ViewResult;

            //Assert
            actual.Should().NotBeNull();
            var model = actual!.Model as SelectTrainingProviderViewModel;
            model.Should().NotBeNull();
            model!.ShortListItemCount.Should().Be(0);
            mockService.Verify(x => x.GetShortlistForUser(It.IsAny<Guid>()), Times.Never);
        }
    }
}
