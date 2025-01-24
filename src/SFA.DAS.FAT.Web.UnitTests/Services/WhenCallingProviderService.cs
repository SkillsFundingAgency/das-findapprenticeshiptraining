using System.Text;
using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Providers.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Domain.Providers.Api;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class WhenCallingProviderService
{
    [Test, MoqAutoData]
    public async Task Then_Results_In_Cache_Returned(
        [Frozen] Mock<IApiClient> outerApiMock,
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] IOptions<FindApprenticeshipTrainingApi> config,
        GetRegisteredProvidersApiRequest request,
        RegisteredProviders registeredProviders
    )
    {
        outerApiMock
            .Setup(o => o.Get<RegisteredProviders>(It.IsAny<GetRegisteredProvidersApiRequest>()))
            .ReturnsAsync(registeredProviders);

        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(registeredProviders.Providers));

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheSetting.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);

        ProviderService providerService = new ProviderService(outerApiMock.Object, cacheService, config);

        var actual = await providerService.GetRegisteredProviders();

        actual.Should().BeEquivalentTo(registeredProviders.Providers);
        mockDistributedCache.Verify(x => x.GetAsync(CacheSetting.Providers.Key, default), Times.Once);
        mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
        outerApiMock.Verify(x => x.Get<RegisteredProvider>(It.IsAny<GetRegisteredProvidersApiRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Results_In_Repository_Returned_And_Cache_Set(
        [Frozen] Mock<IApiClient> outerApiMock,
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] IOptions<FindApprenticeshipTrainingApi> config,
        GetRegisteredProvidersApiRequest request,
        RegisteredProviders registeredProviders
    )
    {
        outerApiMock
            .Setup(o => o.Get<RegisteredProviders>(It.IsAny<GetRegisteredProvidersApiRequest>()))
            .ReturnsAsync(registeredProviders);

        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(registeredProviders.Providers));

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheSetting.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);

        ProviderService providerService = new ProviderService(outerApiMock.Object, cacheService, config);

        var actual = await providerService.GetRegisteredProviders();

        actual.Should().BeEquivalentTo(registeredProviders.Providers);
        mockDistributedCache.Verify(x => x.GetAsync(CacheSetting.Providers.Key, default), Times.Once);
        mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), cachedData, It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        outerApiMock.Verify(x => x.Get<RegisteredProviders>(It.Is<GetRegisteredProvidersApiRequest>(b => b.BaseUrl == config.Value.BaseUrl)), Times.Once);
    }
}
