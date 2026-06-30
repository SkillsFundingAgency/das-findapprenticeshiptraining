using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Extensions;

public class LocationCookieExtentionTests
{
    [Test, MoqAutoData]
    public void GetLocation_WhenCookieMissing_ReturnsDefaultLocationAndDistance(
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieService)
    {
        // Arrange
        cookieService
            .Setup(x => x.Get(Constants.LocationCookieName))
            .Returns((LocationCookieItem)null);

        // Act
        var result = cookieService.Object.GetLocation();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Location, Is.EqualTo(string.Empty));
            Assert.That(result.Distance, Is.EqualTo(DistanceService.DefaultDistance.ToString()));
        }
    }

    [Test, MoqAutoData]
    public void GetLocation_WhenCookiePresent_ReturnsCookieLocationAndDistance(
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieService)
    {
        // Arrange
        var cookieItem = new LocationCookieItem
        {
            Location = "Bristol",
            Distance = "20"
        };

        cookieService
            .Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(cookieItem);

        // Act
        var result = cookieService.Object.GetLocation();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Location, Is.EqualTo(cookieItem.Location));
            Assert.That(result.Distance, Is.EqualTo(cookieItem.Distance));
        }
    }
}
