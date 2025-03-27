using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistsForUser;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Queries;

public class WhenGettingShortlistForUser
{
    [Test, MoqAutoData]
    public async Task Then_Returns_Results_From_Service(
        GetShortlistsForUserQuery query,
        GetShortlistsForUserResponse shortlistFromService,
        [Frozen] Mock<IShortlistService> mockService,
        GetShortlistsForUserQueryHandler handler)
    {
        mockService
            .Setup(service => service.GetShortlistsForUser(query.ShortlistUserId))
            .ReturnsAsync(shortlistFromService);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().Be(shortlistFromService);
    }
}
