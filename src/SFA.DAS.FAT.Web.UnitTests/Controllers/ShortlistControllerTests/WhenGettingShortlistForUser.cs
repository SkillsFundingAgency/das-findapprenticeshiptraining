using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests
{
    public class WhenGettingShortlistForUser
    {
        [Test, MoqAutoData]
        public async Task And_Cookie_Exists_Then_Reads_Cookie_And_Builds_ViewModel_From_Mediator_Result(
            ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingWeb>> config,
            [Greedy] ShortlistController controller)
        {
            //Arrange
            config.Object.Value.EmployerDemandFeatureToggle = false;
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetShortlistForUserQuery>(c => c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultFromMediator);
            
            //Act
            var actual = await controller.Index("") as ViewResult;
            
            //Assert
            actual.Should().NotBeNull();
            var model = actual.Model as ShortlistViewModel;
            model.Should().NotBeNull();
            model.Shortlist.Should()
                .BeEquivalentTo(
                    resultFromMediator.Shortlist.Select(item => (ShortlistItemViewModel)item));
            model.Removed.Should().BeEmpty();
            model.HelpBaseUrl.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Removed_Query_Param_Is_Set_It_Is_Decoded(
            string removed,
            ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] ShortlistController controller)
        {
            //Arrange
            var encodedData = Encoding.UTF8.GetBytes($"{removed}");
            protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(encodedData);
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetShortlistForUserQuery>(c => c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultFromMediator);
            
            //Act
            var actual = await controller.Index(WebEncoders.Base64UrlEncode(encodedData)) as ViewResult;
            
            //Assert
            actual.Should().NotBeNull();
            var model = actual.Model as ShortlistViewModel;
            model.Should().NotBeNull();
            model.Shortlist.Should()
                .BeEquivalentTo(
                    resultFromMediator.Shortlist.Select(item => (ShortlistItemViewModel)item));
            model.Removed.Should().Be(removed);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Encoded_Removed_Param_Throws_A_Cryptographic_Exception_Then_It_Is_Set_To_Empty(
            string removed,
            ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] ShortlistController controller)
        {
            var encodedData = Encoding.UTF8.GetBytes($"{removed}");
            protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Throws<CryptographicException>();
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetShortlistForUserQuery>(c => c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultFromMediator);
            
            //Act
            var actual = await controller.Index(WebEncoders.Base64UrlEncode(encodedData)) as ViewResult;
            
            //Assert
            actual.Should().NotBeNull();
            var model = actual.Model as ShortlistViewModel;
            model.Should().NotBeNull();
            model.Shortlist.Should()
                .BeEquivalentTo(
                    resultFromMediator.Shortlist.Select(item => (ShortlistItemViewModel)item));
            model.Removed.Should().BeEmpty();
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_Encoded_Removed_Param_Is_Invalid_Then_It_Is_Set_To_Empty(
            string removed,
            ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] ShortlistController controller)
        {
            var encodedData = Encoding.UTF8.GetBytes($"{removed}");
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetShortlistForUserQuery>(c => c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultFromMediator);
            
            //Act
            var actual = await controller.Index(encodedData.ToString()) as ViewResult;
            
            //Assert
            actual.Should().NotBeNull();
            var model = actual.Model as ShortlistViewModel;
            model.Should().NotBeNull();
            model.Shortlist.Should()
                .BeEquivalentTo(
                    resultFromMediator.Shortlist.Select(item => (ShortlistItemViewModel)item));
            model.Removed.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task And_Cookie_No_Exists_Then_Returns_Empty_ViewModel(
            ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] ShortlistController controller)
        {
            //Arrange
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(default(ShortlistCookieItem));
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetShortlistForUserQuery>(c => c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultFromMediator);
            
            //Act
            var actual = await controller.Index(null) as ViewResult;
            
            //Assert
            actual.Should().NotBeNull();
            var model = actual.Model as ShortlistViewModel;
            model.Should().NotBeNull();
            model.Shortlist.Should().BeEmpty();
            mockMediator.Verify(mediator => mediator.Send(
                    It.IsAny<GetShortlistForUserQuery>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Help_Link_Is_Shown_If_The_Feature_Is_Toggled_On(ShortlistCookieItem shortlistCookie,
            GetShortlistForUserResult resultFromMediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> mockCookieService,
            [Frozen] Mock<IMediator> mockMediator,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingWeb>> config,
            [Greedy] ShortlistController controller)
        {
            //Arrange
            config.Object.Value.EmployerDemandFeatureToggle = true;
            mockCookieService
                .Setup(service => service.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookie);
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetShortlistForUserQuery>(c => c.ShortlistUserId.Equals(shortlistCookie.ShortlistUserId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultFromMediator);
            
            //Act
            var actual = await controller.Index("") as ViewResult;
            
            //Assert
            actual.Should().NotBeNull();
            var model = actual.Model as ShortlistViewModel;
            model.Should().NotBeNull();
            model.Shortlist.Should()
                .BeEquivalentTo(
                    resultFromMediator.Shortlist.Select(item => (ShortlistItemViewModel)item));
            model.Removed.Should().BeEmpty();
            model.HelpBaseUrl.Should().Be(config.Object.Value.EmployerDemandUrl);
            foreach (var itemViewModel in model.Shortlist)
            {
                itemViewModel.HelpFindingCourseUrl.Should().Be($"/registerdemand/course/{itemViewModel.Course.Id}/share-interest");    
            }
            
        }
    }
}
