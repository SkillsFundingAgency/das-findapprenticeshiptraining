using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests
{
    public class WhenGettingClearKeywordLink
    {
        [Test, AutoData]
        public void Then_No_Filter_Items_Builds_Correct_Clear_Links()
        {
            //Arrange
            var viewModel = new Web.Models.CoursesViewModel();

            //Assert
            viewModel.ClearKeywordLink.Should().BeEmpty();
            viewModel.ClearSectorLinks.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_The_Clear_Keyword_Link_Is_Generated_If_Filtered_By_Keyword(string keyword)
        {
            //Arrange Act
            var model = new Web.Models.CoursesViewModel
            {
                Keyword = keyword,
            };

            //Assert
            model.ClearKeywordLink.Should().NotBeNull();
            model.ClearKeywordLink.Should().Be("");
        }

        [Test, AutoData]
        public void Then_The_Clear_Keyword_Link_Is_Generated_If_Filtered_By_Keyword_With_Sectors(List<string> selectedRoutes, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, new List<int>());

            //Assert
            model.ClearKeywordLink.Should().NotBeNull();
            model.ClearKeywordLink.Should().Be("?sectors=" + string.Join("&sectors=", model.SelectedSectors.Select(HttpUtility.HtmlEncode)));
        }

        [Test, AutoData]
        public void Then_The_Clear_Keyword_Link_Is_Generated_If_Filtered_By_Keyword_With_Levels(List<int> selectedLevels, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<string>(), keyword, selectedLevels);

            //Assert
            model.ClearKeywordLink.Should().NotBeNull();
            model.ClearKeywordLink.Should().Be("?levels=" + string.Join("&levels=", model.SelectedLevels));
        }

        [Test, AutoData]
        public void Then_The_Clear_Keyword_Link_Is_Generated_If_Filtered_By_Keyword_With_Sectors_And_Levels(List<string> selectedRoutes, List<int> selectedLevels, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, selectedLevels);

            //Assert
            model.ClearKeywordLink.Should().NotBeNull();
            model.ClearKeywordLink.Should().Be("?sectors=" + string.Join("&sectors=", model.SelectedSectors.Select(HttpUtility.HtmlEncode)) + "&levels=" + string.Join("&levels=", model.SelectedLevels));
        }
    }
}
