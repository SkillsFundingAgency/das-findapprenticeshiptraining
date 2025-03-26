using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingCourseProviderViewModel
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Correctly_Mapped(GetCourseProviderResult source)
        {
            var actual = (CourseProviderViewModel)source;

            actual.Course.Should().BeEquivalentTo((CourseViewModel)source.Course);
            actual.AdditionalCourses.Should().BeEquivalentTo((ProviderCoursesViewModel)source.AdditionalCourses);
            actual.Provider.Should().BeEquivalentTo((ProviderViewModel)source.Provider);
            actual.ProvidersAtLocation.Should().Be(source.ProvidersAtLocation);
            actual.TotalProviders.Should().Be(source.TotalProviders);
        }

        [Test, AutoData]
        public void Then_If_The_CoreSkills_are_Null_Then_Empty_List_Returned(GetCourseProviderResult source)
        {
            source.Course.CoreSkills = null;

            var actual = (CourseProviderViewModel)source;

            actual.Course.CoreSkills.Should().NotBeNull();
            actual.Course.CoreSkills.Should().BeEmpty();
        }
    }
}
