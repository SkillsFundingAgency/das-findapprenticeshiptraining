using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingLocationViewModel
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(Locations.LocationItem source)
        {
            var actual = (LocationViewModel)source;

            source.Should().BeEquivalentTo(actual);
        }
    }
}
