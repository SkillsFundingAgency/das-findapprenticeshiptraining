using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models;
public class WhenCreatingSelectTrainingProviderViewModel
{
    [Test, AutoData]
    public void Then_The_Fields_Are_Mapped(int shortlistItemCount)
    {
        var vm = new SelectTrainingProviderViewModel(shortlistItemCount);

        using (new AssertionScope())
        {
            vm.ShortListItemCount.Should().Be(shortlistItemCount);
            vm.ShowBackLink.Should().BeTrue();
            vm.ShowSearchCrumb.Should().BeFalse();
            vm.ShowShortListLink.Should().BeTrue();
        }
    }
}
