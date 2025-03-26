using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models;
public class WhenCreatingSelectTrainingProviderViewModel
{
    [Test, AutoData]
    public void Then_The_Fields_Are_Mapped()
    {
        var vm = new SelectTrainingProviderViewModel();

        using (new AssertionScope())
        {
            vm.ShowSearchCrumb.Should().BeTrue();
            vm.ShowShortListLink.Should().BeTrue();
        }
    }
}
