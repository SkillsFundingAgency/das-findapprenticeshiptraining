using System.Text;
using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.RegisteredProvidersControllerTests;
public class WhenGettingRegisteredProviders
{
    private const string LongQuery = "query";
    private const string UkprnSearchString = "100";
    private const int UkprnSeed = 1000000;

    [Test, MoqAutoData]

    public async Task Then_The_Query_Is_Sent_And_Data_From_Cache_Retrieved_Matched_By_Name(
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] Mock<IProviderService> mockProviderService,
        List<RegisteredProvider> registeredProviders,
        CancellationToken cancellationToken
        )
    {
        //Arrange
        foreach (var provider in registeredProviders)
        {
            provider.Name = LongQuery + provider.Name;
        }

        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(registeredProviders));

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheStorageValues.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);
        var controller = new RegisteredProvidersController(cacheService, mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(LongQuery, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(registeredProviders);
        mockDistributedCache.Verify(x => x.GetAsync(CacheStorageValues.Providers.Key, default), Times.Once);
        mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_From_Cache_Retrieved_Matched_By_Ukprn(
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] Mock<IProviderService> mockProviderService,
        CancellationToken cancellationToken
    )
    {
        //Arrange
        int numberOfMockedRegisteredProviders = 10;
        var registeredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            registeredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = UkprnSeed + i });
        }

        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(registeredProviders));

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheStorageValues.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);
        var controller = new RegisteredProvidersController(cacheService, mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(UkprnSearchString, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(registeredProviders);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_From_Repository_Retrieved_Matched_By_Name_And_Set_Cache(
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] Mock<IProviderService> mockProviderService,
        List<RegisteredProvider> registeredProviders,
        CancellationToken cancellationToken
    )
    {
        //Arrange
        foreach (var provider in registeredProviders)
        {
            provider.Name = LongQuery + provider.Name;
        }

        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(registeredProviders));

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheStorageValues.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(registeredProviders);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);
        var controller = new RegisteredProvidersController(cacheService, mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(LongQuery, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(registeredProviders);
        mockDistributedCache.Verify(x => x.GetAsync(CacheStorageValues.Providers.Key, default), Times.Once);
        mockDistributedCache.Verify(x => x.SetAsync(CacheStorageValues.Providers.Key, cachedData, It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_From_Repository_Retrieved_Matched_By_Ukprn(
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] Mock<IProviderService> mockProviderService,
        CancellationToken cancellationToken
    )
    {
        //Arrange
        int numberOfMockedRegisteredProviders = 10;
        var registeredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            registeredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = UkprnSeed + i });
        }

        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(registeredProviders));

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheStorageValues.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(registeredProviders);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);
        var controller = new RegisteredProvidersController(cacheService, mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(UkprnSearchString, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(registeredProviders);
        mockDistributedCache.Verify(x => x.GetAsync(CacheStorageValues.Providers.Key, default), Times.Once);
        mockDistributedCache.Verify(x => x.SetAsync(CacheStorageValues.Providers.Key, cachedData, It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Once);
    }


    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_For_First_100_Records(
        [Frozen] Mock<IDistributedCache> mockDistributedCache,
        [Frozen] Mock<IProviderService> mockProviderService,
        CancellationToken cancellationToken
    )
    {
        // Arrange
        int numberOfMockedRegisteredProviders = 200;
        int numberOfExpectedRegisteredProviders = 100;
        var mockedRegisteredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            mockedRegisteredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = UkprnSeed + i });
        }

        mockDistributedCache
            .Setup(x => x.GetAsync(CacheStorageValues.Providers.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(mockedRegisteredProviders);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);
        var controller = new RegisteredProvidersController(cacheService, mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(UkprnSearchString, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Count.Should().Be(numberOfExpectedRegisteredProviders);
        returnedProviders.Should().BeEquivalentTo(mockedRegisteredProviders.Take(numberOfExpectedRegisteredProviders));

    }

    [TestCase(null)]
    [TestCase("a")]
    [TestCase("ab")]
    [TestCase(" AB ")]
    [TestCase("")]
    public async Task Then_Return_No_Results_Where_SearchTerm_Is_Less_Than_3_Characters(string? searchTerm)
    {
        //Arrange
        Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
        Mock<IProviderService> mockProviderService = new Mock<IProviderService>();

        int numberOfMockedRegisteredProviders = 10;
        var mockedRegisteredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            mockedRegisteredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = UkprnSeed + i });
        }

        // mockDistributedCache
        //     .Setup(x => x.GetAsync(CacheStorageValues.Providers.Key, It.IsAny<CancellationToken>()))
        //     .ReturnsAsync((byte[])null!);
        //
        // mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(mockedRegisteredProviders);

        var cacheService = new DistributedCacheService(mockDistributedCache.Object);
        var controller = new RegisteredProvidersController(cacheService, mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(searchTerm, new CancellationToken());
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Count.Should().Be(0);

        mockDistributedCache.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Never);
    }
}
