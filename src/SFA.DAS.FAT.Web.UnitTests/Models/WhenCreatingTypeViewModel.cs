using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenCreatingTypeViewModel
{
    [Test, AutoData]
    public void Then_The_Values_Are_Mapped_Correctly(ApprenticeType type)
    {
        var actual = new TypeViewModel(type, null);

        actual.Should().BeEquivalentTo(type);
    }

    [Test, AutoData]
    public void Then_Any_Selected_Ids_Are_Marked_As_Selected(ApprenticeType route)
    {
        var actual = new TypeViewModel(route, new List<string> { route.Name });

        actual.Selected.Should().BeTrue();
    }
}
