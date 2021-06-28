using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingSectorViewModel
    {
        [Test, AutoData]
        public void Then_The_Values_Are_Mapped_Correctly(Sector sector)
        {
            var actual = new SectorViewModel(sector, null);
            
            actual.Should().BeEquivalentTo(sector);
        }

        [Test, AutoData]
        public void Then_Any_Selected_Ids_Are_Marked_As_Selected(Sector sector)
        {
            var actual = new SectorViewModel(sector, new List<string>{sector.Route});

            actual.Selected.Should().BeTrue();
        }
    }
}