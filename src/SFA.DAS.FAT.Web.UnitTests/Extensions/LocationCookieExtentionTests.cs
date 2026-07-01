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
        var (Location, Distance) = LocationCookieExtention.GetLocation(cookieService.Object);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(Location, Is.EqualTo(string.Empty));
            Assert.That(Distance, Is.EqualTo(DistanceService.DefaultDistance.ToString()));
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
        var (Location, Distance) = LocationCookieExtention.GetLocation(cookieService.Object);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(Location, Is.EqualTo(cookieItem.Location));
            Assert.That(Distance, Is.EqualTo(cookieItem.Distance));
        }
    }
}
