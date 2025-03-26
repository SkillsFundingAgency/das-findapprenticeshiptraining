using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Commands;

public class WhenCreatingShortlistItemForUser
{
    [Test, MoqAutoData]
    public async Task Then_The_Command_Is_Handled_And_Service_Called(
        Guid expectedId,
        CreateShortlistItemForUserCommand command,
        [Frozen] Mock<IShortlistService> service,
        CreateShortlistItemForUserCommandHandler sut)
    {
        //Arrange
        service.Setup(x => x.CreateShortlistItemForUser(command)).ReturnsAsync(expectedId);

        //Act
        var actual = await sut.Handle(command, CancellationToken.None);

        //Assert
        actual.Should().Be(expectedId);
    }
}
