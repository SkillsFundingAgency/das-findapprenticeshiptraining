using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Providers.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Providers.Api;
public class WhenCreatingTheGetRegisteredProvidersApiRequest
{
    [Test, AutoData]
    public void Then_The_Get_Url_Is_Constructed_Correctly(string baseUrl)
    {
        //Arrange Act
        var actual = new GetRegisteredProvidersApiRequest(baseUrl);
        //Assert
        actual.GetUrl.Should().Be($"{baseUrl}providers");
    }
}
