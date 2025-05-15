using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Standards.Queries.GetStandards;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.UnitTests.Standards.Queries.GetStandards;

public class GetStandardsQueryHandlerTests
{
    [Test, AutoData]
    public async Task Handle_GetsStandardsFromOuterApi(GetStandardsQueryResult expectedResult)
    {
        var query = new GetStandardsQuery();
        var mockApiClient = new Mock<IApiClient>();
        mockApiClient
            .Setup(x => x.Get<GetStandardsQueryResult>(It.IsAny<IGetApiRequest>()))
            .ReturnsAsync(expectedResult);
        var handler = new GetStandardsQueryHandler(mockApiClient.Object, Options.Create(new FindApprenticeshipTrainingApi()));
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
