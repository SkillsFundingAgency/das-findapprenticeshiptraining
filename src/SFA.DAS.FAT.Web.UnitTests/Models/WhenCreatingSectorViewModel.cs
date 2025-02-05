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
        public void Then_The_Values_Are_Mapped_Correctly(Route route)
        {
            var actual = new RouteViewModel(route, null);
            
            actual.Should().BeEquivalentTo(route);
        }

        [Test, AutoData]
        public void Then_Any_Selected_Ids_Are_Marked_As_Selected(Route route)
        {
            var actual = new RouteViewModel(route, new List<string>{ route.Name });

            actual.Selected.Should().BeTrue();
        }
    }
}
