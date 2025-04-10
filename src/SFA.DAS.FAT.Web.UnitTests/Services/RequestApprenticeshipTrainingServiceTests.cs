using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class RequestApprenticeshipTrainingServiceTests
{
    [Test]
    public void GetRequestApprenticeshipTrainingUrl_WhenCalledWithLocation_ThenShouldReturnUrlWithLocation()
    {
        // Arrange
        var expected = "https://test.com/service/?redirectUri=https%3A%2F%2Frat.com%2Faccounts%2F%7B%7BhashedAccountId%7D%7D%2Femployer-requests%2Foverview%3FstandardId%3D1%26requestType%3DShortlist%26location%3Dlocation";
        var config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://rat.com",
            EmployerAccountsUrl = "https://test.com"
        };
        var service = new RequestApprenticeshipTrainingService(config);
        // Act
        var result = service.GetRequestApprenticeshipTrainingUrl(1, EntryPoint.Shortlist, "location");
        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void GetRequestApprenticeshipTrainingUrl_WhenCalledWithEmptyLocation_ThenShouldReturnUrlWithOutLocation()
    {
        // Arrange
        var expected = "https://test.com/service/?redirectUri=https%3A%2F%2Frat.com%2Faccounts%2F%7B%7BhashedAccountId%7D%7D%2Femployer-requests%2Foverview%3FstandardId%3D1%26requestType%3DShortlist";
        var config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://rat.com",
            EmployerAccountsUrl = "https://test.com"
        };
        var service = new RequestApprenticeshipTrainingService(config);
        // Act
        var result = service.GetRequestApprenticeshipTrainingUrl(1, EntryPoint.Shortlist, string.Empty);
        // Assert
        result.Should().Be(expected);
    }
}
