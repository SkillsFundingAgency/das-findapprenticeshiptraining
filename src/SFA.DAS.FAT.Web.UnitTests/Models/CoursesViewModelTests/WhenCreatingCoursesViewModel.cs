using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests
{
    public class WhenCreatingCoursesViewModel
    {
        [TestCase(10, 5, "", "10 results")]
        [TestCase(10, 5, "test", "5 results")]
        [TestCase(1, 5, "", "1 result")]
        [TestCase(5, 1, "test", "1 result")]
        [TestCase(0, 5, "", "0 results")]
        [TestCase(5, 0, "test", "0 results")]
        public void Then_The_Total_Message_Is_Created_Correctly(int totalCount, int filterTotal, string keyword, string expectedMessage)
        {
            var viewModel = new Web.Models.CoursesViewModel
            {
                Total = totalCount,
                TotalFiltered = filterTotal,
                Keyword = keyword
            };

            viewModel.TotalMessage.Should().Be(expectedMessage);
        }

        [Test, AutoData]
        public void Then_The_Total_Message_Uses_Filtered_Total_If_There_Are_Selected_Filters()
        {
            var viewModel = new Web.Models.CoursesViewModel
            {
                Total = 10,
                TotalFiltered = 5,
                SelectedRoutes = new List<string> { "" }
            };

            viewModel.TotalMessage.Should().Be("5 results");
        }

        [Test, AutoData]
        public void Then_The_Total_Message_Uses_Filtered_Total_If_There_Are_Selected_Levels()
        {
            var viewModel = new Web.Models.CoursesViewModel
            {
                Total = 10,
                TotalFiltered = 5,
                SelectedLevels = new List<int> { 1 }
            };

            viewModel.TotalMessage.Should().Be("5 results");
        }

        [Test, AutoData]
        public void Then_The_Total_Message_Uses_Filtered_Total_If_There_Are_Selected_Levels_Sectors_And_Keyword()
        {
            var viewModel = new Web.Models.CoursesViewModel
            {
                Total = 10,
                TotalFiltered = 5,
                SelectedLevels = new List<int> { 1 },
                SelectedRoutes = new List<string> { "" },
                Keyword = "Test"
            };

            viewModel.TotalMessage.Should().Be("5 results");
        }

        [Test]
        public void Then_If_There_Is_A_Keyword_Search_And_No_OrderBy_It_Is_Set_To_Relevance()
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<string>(), "test", new List<int>(), OrderBy.None);

            //Assert
            model.OrderBy.Should().Be(OrderBy.Relevance);
        }

        [Test]
        public void Then_If_There_Is_A_Keyword_Search_And_OrderBy_It_Is_Maintained()
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<string>(), "test", new List<int>());

            //Assert
            model.OrderBy.Should().Be(OrderBy.Name);
        }

        [Test]
        public void Then_If_There_Is_No_Keyword_Search_And_OrderBy_It_Is_Set_To_None()
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<string>(), "", new List<int>());

            //Assert
            model.OrderBy.Should().Be(OrderBy.None);
        }
    }
}
