using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ShortlistsViewModelTests;

public class WhenCreatingShortlistsViewModel
{
    [TestCase("provider name", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void ThenShowRemovedShortlistBannerIsAsExpected(string removedProviderName, bool expected)
    {
        ShortlistsViewModel sut = new()
        {
            RemovedProviderName = removedProviderName
        };

        sut.ShowRemovedShortlistBanner.Should().Be(expected);
    }
}
