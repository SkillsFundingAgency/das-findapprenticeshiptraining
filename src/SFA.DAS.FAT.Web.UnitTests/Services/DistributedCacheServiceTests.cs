using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class DistributedCacheServiceTests
{
    private Mock<IDistributedCache> _mockDistributedCache;
    private Mock<IMemoryCache> _mockMemoryCache;
    private DistributedCacheService _distributedCacheService;

    [SetUp]
    public void Setup()
    {
        _mockDistributedCache = new Mock<IDistributedCache>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _distributedCacheService = new DistributedCacheService(_mockDistributedCache.Object, _mockMemoryCache.Object);
    }

    [Test]
    public async Task GetOrSetAsync_ShouldReturnDistributedCachedData_WhenDataExistsInDistributedCache()
    {
        // Arrange
        string cacheKey = "testKey";
        string cachedValue = "Cached Value";
        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedValue));
        _mockDistributedCache
            .Setup(x => x.GetAsync(cacheKey, default))
            .ReturnsAsync(cachedData);

        // Act
        var result = await _distributedCacheService.GetOrSetAsync(
            cacheKey,
            () => Task.FromResult("New Value"),
            TimeSpan.FromMinutes(10));

        // Assert
        result.Should().Be(cachedValue);
        _mockDistributedCache.Verify(x => x.GetAsync(cacheKey, default), Times.Once);
        _mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
    }

    [Test]
    public async Task GetOrSetAsync_ShouldReturnMemoryCachedData_WhenDataExistsInMemoryCache()
    {
        string cacheKey = "testKey";
        string cachedValue = "Cached Value";
        var cachedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedValue));
        _mockMemoryCache
            .Setup(x => x.Get(cacheKey))
            .Returns(cachedData);

        var result = await _distributedCacheService.GetOrSetAsync(
            cacheKey,
            () => Task.FromResult("New Value"),
            TimeSpan.FromMinutes(10));

        result.Should().Be(cachedValue);
        _mockMemoryCache.Verify(x => x.Get(cacheKey), Times.Once);
        _mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
    }

    [Test]
    public async Task GetOrSetAsync_ShouldSetAndReturnNewData_WhenCacheIsEmpty()
    {
        // Arrange
        string cacheKey = "testKey";
        string value = "New Value";
        byte[] valueInBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        _mockDistributedCache.Setup(x => x.GetAsync(cacheKey, default)).ReturnsAsync((byte[])null);

        // Act
        var result = await _distributedCacheService.GetOrSetAsync(
            cacheKey,
            () => Task.FromResult(value),
            TimeSpan.FromMinutes(10));

        // Assert
        result.Should().Be(value);

        _mockDistributedCache.Verify(x => x.GetAsync(cacheKey, default), Times.Once);
        _mockDistributedCache.Verify(x => x.SetAsync(
            cacheKey,
            valueInBytes,
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RemoveAsync_ShouldRemoveItemFromCache()
    {
        // Arrange
        string cacheKey = "testKey";

        // Act
        await _distributedCacheService.RemoveAsync(cacheKey);

        // Assert
        _mockDistributedCache.Verify(x => x.RemoveAsync(cacheKey, default), Times.Once);
        _mockMemoryCache.Verify(x => x.Remove(cacheKey), Times.Once);
    }
}

