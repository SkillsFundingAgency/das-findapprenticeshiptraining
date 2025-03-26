using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Services;

public class WhenGettingShortlistsCountForUser
{
    [Test, AutoData]
    public async Task Then_The_Count_Is_Returned_From_The_Api(
        Guid shortlistUserId,
        FindApprenticeshipTrainingApi config,
        ShortlistsCount apiResponse)
    {
        //Arrange
        Mock<IApiClient> apiClientMock = new();
        Mock<ISessionService> sessionServiceMock = new();
        var sut = new ShortlistService(apiClientMock.Object, Options.Create(config), sessionServiceMock.Object);
        sessionServiceMock.Setup(x => x.Get<ShortlistsCount>()).Returns((ShortlistsCount)null);
        apiClientMock
            .Setup(x => x.Get<ShortlistsCount>(It.Is<GetShortlistsCountForUserRequest>(c => c.GetUrl.Contains(shortlistUserId.ToString()))))
            .ReturnsAsync(apiResponse);
        //Act
        var actual = await sut.GetShortlistsCountForUser(shortlistUserId);
        //Assert
        actual.Should().Be(apiResponse.Count);
        sessionServiceMock.Verify(x => x.Set(apiResponse), Times.Once);
    }

    [Test, AutoData]
    public async Task Then_The_Count_Is_Returned_From_Session(ShortlistsCount shortlistsCount)
    {
        //Arrange
        Mock<IApiClient> apiClientMock = new();
        Mock<ISessionService> sessionServiceMock = new();
        var sut = new ShortlistService(apiClientMock.Object, Options.Create(new FindApprenticeshipTrainingApi()), sessionServiceMock.Object);
        sessionServiceMock.Setup(x => x.Get<ShortlistsCount>()).Returns(shortlistsCount);
        //Act
        var actual = await sut.GetShortlistsCountForUser(Guid.NewGuid());
        //Assert
        actual.Should().Be(shortlistsCount.Count);
        apiClientMock.Verify(x => x.Get<ShortlistsCount>(It.IsAny<GetShortlistsCountForUserRequest>()), Times.Never);
    }
}
