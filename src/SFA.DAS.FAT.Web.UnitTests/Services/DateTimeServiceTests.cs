using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class DateTimeServiceTests
{
    [Test]
    public void Then_The_Date_Is_Returned()
    {
        var service = new DateTimeService();

        var actual = service.GetDateTime();

        actual.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 0, 0, 5));
    }
}
