using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Services;

public class WhenGettingShortlistForUser
{
    [Test, MoqAutoData]
    public async Task Then_The_Data_Is_Returned_From_The_Api(
        Guid shortlistUserId,
        GetShortlistsForUserResponse apiResponse,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService sut)
    {
        //Arrange
        apiClient.Setup(x =>
                x.Get<GetShortlistsForUserResponse>(
                    It.Is<GetShortlistsForUserRequest>(c => c.GetUrl.Contains(shortlistUserId.ToString(), StringComparison.InvariantCultureIgnoreCase))))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await sut.GetShortlistsForUser(shortlistUserId);

        //Assert
        actual.Should().BeEquivalentTo(apiResponse);
    }
}
