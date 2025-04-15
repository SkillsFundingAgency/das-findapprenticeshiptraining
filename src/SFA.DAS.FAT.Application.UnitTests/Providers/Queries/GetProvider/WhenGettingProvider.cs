using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Providers.Query.GetProvider;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers.Api.Requests;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Providers.Queries.GetProvider;
public class WhenGettingProvider
{
    private const string BaseUrl = "https://test.local/";

    [Test, MoqAutoData]
    public async Task Then_Returns_Results(
        GetProviderQuery query,
        GetProviderQueryResponse response,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient)
    {
        config.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi()
        {
            BaseUrl = BaseUrl
        });

        apiClient.Setup(x => x.Get<GetProviderQueryResponse>(
                    It.Is<GetProviderApiRequest>(r =>
                        r.BaseUrl == BaseUrl
                    )
                )
            )
            .ReturnsAsync(response);

        var handler = new GetProviderQueryHandler(apiClient.Object, config.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(response);

        apiClient.Verify(x => x.Get<GetProviderQueryResponse>(It.IsAny<GetProviderApiRequest>()), Times.Once);

    }
}
