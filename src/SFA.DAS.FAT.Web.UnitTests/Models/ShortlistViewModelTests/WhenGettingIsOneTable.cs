using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ShortlistViewModelTests
{
    public class WhenGettingIsOneTable
    {
        [Test, AutoData]
        public void And_Greater_Than_One_CourseTitle_Then_False(ShortlistViewModel model)
        {
            foreach (var item in model.Shortlist)
            {
                item.Course.Level = 1;
                item.LocationDescription = "the same location";
            }

            model.IsOneTable.Should().BeFalse();
        }

        [Test, AutoData]
        public void And_Greater_Than_One_CourseLevel_Then_False(ShortlistViewModel model)
        {
            foreach (var item in model.Shortlist)
            {
                item.Course.Title = "the same course title";
                item.LocationDescription = "the same location";
            }

            model.IsOneTable.Should().BeFalse();
        }

        [Test, AutoData]
        public void And_Greater_Than_One_Location_Then_False(ShortlistViewModel model)
        {
            foreach (var item in model.Shortlist)
            {
                item.Course.Title = "the same course title";
                item.Course.Level = 1;
            }

            model.IsOneTable.Should().BeFalse();
        }

        [Test, AutoData]
        public void And_One_Title_And_One_Level_And_One_Location_Then_True(ShortlistViewModel model)
        {
            foreach (var item in model.Shortlist)
            {
                item.Course.Title = "the same course title";
                item.Course.Level = 1;
                item.LocationDescription = "the same location";
            }

            model.IsOneTable.Should().BeTrue();
        }
    }
}
