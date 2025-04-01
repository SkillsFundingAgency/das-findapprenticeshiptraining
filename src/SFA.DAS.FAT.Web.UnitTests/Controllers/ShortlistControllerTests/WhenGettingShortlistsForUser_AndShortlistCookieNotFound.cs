using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ShortlistControllerTests;

public class WhenGettingShortlistsForUser_AndShortlistCookieNotFound
{
    [Test, MoqAutoData]
    public async Task ThenReturnsEmptyViewModel(
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ShortlistController sut,
        Mock<ITempDataDictionary> tempDataMock)
    {
        //Arrange
        shortlistCookieServiceMock.Setup(s => s.Get(Constants.ShortlistCookieName)).Returns(() => null);
        //Act
        var response = await sut.Index();
        //Assert
        shortlistCookieServiceMock.Verify(x => x.Get(Constants.ShortlistCookieName), Times.Once);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetShortlistsForUserQuery>(), default), Times.Never);
        response.As<ViewResult>().Model.As<ShortlistsViewModel>().HasShortlistItems.Should().BeFalse();
    }
}
