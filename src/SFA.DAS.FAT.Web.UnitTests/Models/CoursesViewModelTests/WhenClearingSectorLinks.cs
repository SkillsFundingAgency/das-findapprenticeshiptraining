using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests
{
    public class WhenClearingSectorLinks
    {
        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Sectors(List<string> selectedRoutes, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, new List<int>());

            //Assert
            var clearLinkCount = selectedRoutes.Count;
            model.ClearSectorLinks.Count.Should().Be(clearLinkCount);

            foreach (var selectedRoute in selectedRoutes)
            {
                var sector = model.Sectors.SingleOrDefault(c => c.Route.Equals(selectedRoute));

                sector.Should().NotBeNull();
                model.ClearSectorLinks.ContainsKey(sector!.Route).Should().BeTrue();
                model.ClearSectorLinks.Count(c => c.Value.Contains($"sectors={HttpUtility.HtmlEncode(selectedRoute)}")).Should().Be(clearLinkCount - 1);
                model.ClearSectorLinks.Count(c => c.Value.Contains($"?keyword={keyword}")).Should().Be(clearLinkCount);
            }
        }

        [Test, AutoData]
        public void Then_If_The_Sector_Does_Not_Exist_It_Is_Not_Added(List<string> selectedRoutes)
        {
            //Arrange
            var sectors = selectedRoutes.Take(1)
                .Select(selectedRoute => new RouteViewModel(
                    new Route
                    {
                        Route = selectedRoute
                    }, null))
                .ToList();

            //Act
            var model = new CoursesViewModel
            {
                Sectors = sectors,
                Levels = null,
                Keyword = "",
                SelectedSectors = selectedRoutes,
                SelectedLevels = null,
                OrderBy = OrderBy.Name
            };

            model.ClearSectorLinks.Count.Should().Be(1);
        }

        [Test]
        public void Then_If_A_List_Containing_A_Null_Value_Is_Passed_For_Sectors_Then_Nothing_Is_Added()
        {
            //Act
            var model = new CoursesViewModel
            {
                Sectors = null,
                Levels = null,
                Keyword = "",
                SelectedSectors = new List<string> { null },
                SelectedLevels = null,
                OrderBy = OrderBy.Name
            };

            //Assert
            model.ClearSectorLinks.Should().BeEmpty();
        }
    }
}
