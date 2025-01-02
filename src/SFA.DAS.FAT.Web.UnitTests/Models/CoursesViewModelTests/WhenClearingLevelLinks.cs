using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests
{
    public class WhenClearingLevelLinks
    {
        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Levels(Generator<int> selectedLevelsGenerator)
        {
            //Arrange Act
            var selectedLevels = selectedLevelsGenerator.Distinct().Take(3).ToList();

            var model = CoursesViewModelFactory.BuildModel(new List<string>(), "", selectedLevels);

            //Assert
            var clearLinkCount = selectedLevels.Count;
            model.ClearLevelLinks.Count.Should().Be(clearLinkCount);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                levelViewModel.Should().NotBeNull();
                model.ClearLevelLinks.ContainsKey(levelViewModel!.Title).Should().BeTrue();

                AssertClearLevelLink(model, clearLinkCount);
                model.ClearLevelLinks.Count(c => c.Value.Contains("orderby=")).Should().Be(0);
            }
        }

        [Test, AutoData]
        public void Then_Has_Keyword_Then_Builds_QueryString_With_Keyword(Generator<int> selectedLevelsGenerator, string keyword)
        {
            //Arrange Act
            var selectedLevels = selectedLevelsGenerator.Distinct().Take(3).ToList();

            var model = CoursesViewModelFactory.BuildModel(new List<string>(), keyword, selectedLevels);

            //Assert
            var clearLinkCount = selectedLevels.Count;
            model.ClearLevelLinks.Count.Should().Be(clearLinkCount);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                levelViewModel.Should().NotBeNull();
                model.ClearLevelLinks.ContainsKey(levelViewModel!.Title).Should().BeTrue();

                AssertClearLevelLink(model, clearLinkCount);

                model.ClearLevelLinks.Count(c => c.Value.Contains($"keyword={keyword}")).Should().Be(clearLinkCount);
                model.ClearLevelLinks.Count(c => c.Value.Contains("orderby=")).Should().Be(clearLinkCount);
            }
        }

        [Test, AutoData]
        public void Then_Has_Keyword_And_OrderBy_Then_Builds_QueryString_With_Keyword_And_OrderBy(Generator<int> selectedLevelsGenerator, string keyword)
        {
            //Arrange Act
            var selectedLevels = selectedLevelsGenerator.Distinct().Take(3).ToList();

            var model = CoursesViewModelFactory.BuildModel(new List<string>(), keyword, selectedLevels);

            //Assert
            var clearLinkCount = selectedLevels.Count;
            model.ClearLevelLinks.Count.Should().Be(clearLinkCount);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                levelViewModel.Should().NotBeNull();
                model.ClearLevelLinks.ContainsKey(levelViewModel!.Title).Should().BeTrue();

                AssertClearLevelLink(model, clearLinkCount);

                model.ClearLevelLinks.Count(c => c.Value.Contains($"keyword={keyword}")).Should().Be(clearLinkCount);
                model.ClearLevelLinks.Count(c => c.Value.Contains($"orderby={OrderBy.Name}")).Should().Be(clearLinkCount);
            }
        }

        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Levels_And_Sectors(Generator<int> selectedLevelsGenerator, List<string> selectedRoutes, string keyword)
        {
            //Arrange Act
            var selectedLevels = selectedLevelsGenerator.Distinct().Take(3).ToList();

            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, selectedLevels);

            //Assert
            var clearSectorsLinkCount = selectedRoutes.Count;
            model.ClearSectorLinks.Count.Should().Be(clearSectorsLinkCount);
            var clearLevelsLinkCount = selectedLevels.Count;
            model.ClearLevelLinks.Count.Should().Be(clearLevelsLinkCount);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                levelViewModel.Should().NotBeNull();
                model.ClearLevelLinks.ContainsKey(levelViewModel!.Title).Should().BeTrue();

                AssertClearLevelLink(model, clearLevelsLinkCount);

                model.ClearLevelLinks.Count(c => c.Value.Contains($"keyword={keyword}")).Should().Be(clearLevelsLinkCount);
                model.ClearLevelLinks.Count(c => c.Value.Contains("&sectors=" + string.Join("&sectors=", selectedRoutes.Select(HttpUtility.HtmlEncode)))).Should().Be(clearLevelsLinkCount);

            }
            foreach (var selectedRoute in selectedRoutes)
            {
                var sector = model.Sectors.SingleOrDefault(c => c.Route.Equals(selectedRoute));

                sector.Should().NotBeNull();
                model.ClearSectorLinks.ContainsKey(sector!.Route).Should().BeTrue();
                model.ClearSectorLinks.Count(c => c.Value.Contains($"sectors={HttpUtility.HtmlEncode(selectedRoute)}")).Should().Be(clearSectorsLinkCount - 1);

                model.ClearSectorLinks.Count(c => c.Value.Contains($"?keyword={keyword}")).Should().Be(clearSectorsLinkCount);
                model.ClearSectorLinks.Count(c => c.Value.Contains("&levels=" + string.Join("&levels=", model.SelectedLevels))).Should().Be(clearSectorsLinkCount);
                model.ClearSectorLinks.Count(c => c.Value.Contains("orderby=")).Should().Be(clearSectorsLinkCount);
            }
        }


        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Levels_And_Sectors_And_OrderBy(Generator<int> selectedLevelsGenerator, List<string> selectedRoutes, string keyword)
        {
            //Arrange Act
            var selectedLevels = selectedLevelsGenerator.Distinct().Take(3).ToList();

            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, selectedLevels);

            //Assert
            var clearSectorsLinkCount = selectedRoutes.Count;
            model.ClearSectorLinks.Count.Should().Be(clearSectorsLinkCount);

            var clearLevelsLinkCount = selectedLevels.Count;
            model.ClearLevelLinks.Count.Should().Be(clearLevelsLinkCount);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                levelViewModel.Should().NotBeNull();
                model.ClearLevelLinks.ContainsKey(levelViewModel!.Title).Should().BeTrue();

                AssertClearLevelLink(model, clearLevelsLinkCount);

                model.ClearLevelLinks.Count(c => c.Value.Contains($"?keyword={keyword}")).Should().Be(clearLevelsLinkCount);
                model.ClearLevelLinks.Count(c => c.Value.Contains($"&orderby={OrderBy.Name}")).Should().Be(clearLevelsLinkCount);
                model.ClearLevelLinks.Count(c => c.Value.Contains("&sectors=" + string.Join("&sectors=", model.SelectedSectors.Select(HttpUtility.HtmlEncode)))).Should().Be(clearLevelsLinkCount);
            }

            foreach (var selectedRoute in selectedRoutes)
            {
                var sector = model.Sectors.SingleOrDefault(c => c.Route.Equals(selectedRoute));

                sector.Should().NotBeNull();
                model.ClearSectorLinks.ContainsKey(sector!.Route).Should().BeTrue();
                model.ClearSectorLinks.Count(c => c.Value.Contains($"sectors={HttpUtility.HtmlEncode(selectedRoute)}")).Should().Be(clearSectorsLinkCount - 1);

                model.ClearSectorLinks.Count(c => c.Value.Contains($"?keyword={keyword}")).Should().Be(clearSectorsLinkCount);
                model.ClearSectorLinks.Count(c => c.Value.Contains("&levels=" + string.Join("&levels=", model.SelectedLevels))).Should().Be(clearSectorsLinkCount);
                model.ClearSectorLinks.Count(c => c.Value.Contains("orderby=")).Should().Be(clearSectorsLinkCount);
            }
        }

        [Test, AutoData]
        public void Then_If_The_Level_Does_Not_Exist_It_Is_Not_Added(List<int> selectedLevels)
        {
            //Arrange
            var fixture = new Fixture();
            var levels = selectedLevels.Take(1)
                .Select(selectedLevel => new LevelViewModel(
                    new Level
                    {
                        Code = selectedLevel,
                        Name = fixture.Create<string>()
                    }, null))
                .ToList();

            //Act
            var model = new CoursesViewModel
            {
                Sectors = null,
                Levels = levels,
                Keyword = "",
                SelectedSectors = null,
                SelectedLevels = selectedLevels,
                OrderBy = OrderBy.Name
            };

            model.ClearLevelLinks.Count.Should().Be(1);
        }

        [Test]
        public void Then_If_A_Null_List_Is_Passed_For_Levels_Then_Nothing_Is_Added()
        {
            //Arrange

            //Act
            var model = new CoursesViewModel
            {
                Sectors = null,
                Levels = null,
                Keyword = "",
                SelectedSectors = null,
                SelectedLevels = null,
                OrderBy = OrderBy.Name
            };

            //Assert
            model.ClearLevelLinks.Should().BeEmpty();
        }

        [Test]
        public void Then_If_An_Empty_List_Is_Passed_For_Levels_Then_Nothing_Is_Added()
        {
            //Arrange

            //Act
            var model = new CoursesViewModel
            {
                Sectors = null,
                Levels = null,
                Keyword = "",
                SelectedSectors = null,
                SelectedLevels = new List<int>(),
                OrderBy = OrderBy.Name
            };

            //Assert
            model.ClearLevelLinks.Should().BeEmpty();
        }

        private static void AssertClearLevelLink(CoursesViewModel model, int clearLinkCount)
        {
            foreach (var modelClearLevelLink in model.ClearLevelLinks)
            {
                var queryParams = HttpUtility.ParseQueryString(
                        new Uri("https://test.com/" + modelClearLevelLink.Value).Query)["Levels"].Split(",");
                queryParams.Length.Should().Be(clearLinkCount - 1);
            }
        }
    }
}
