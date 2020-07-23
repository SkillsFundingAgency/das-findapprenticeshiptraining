﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests
{
    public class WhenClearingLevelLinks
    {
        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Levels(List<int> selectedLevels, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<Guid>(), "", selectedLevels);

            //Assert
            var clearLinkCount = selectedLevels.Count;
            Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                Assert.IsNotNull(levelViewModel);
                Assert.IsTrue(model.ClearLevelLinks.ContainsKey(levelViewModel.Title));
                Assert.AreEqual(clearLinkCount - 1, model.ClearLevelLinks.Count(c => c.Value.Contains($"levels={selectedLevel}")));
                Assert.AreEqual(0, model.ClearLevelLinks.Count(c => c.Value.Contains("orderby=")));
            }
            
        }

        
        [Test, AutoData]
        public void Then_Has_Keyword_Then_Builds_QueryString_With_Keyword(List<int> selectedLevels, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<Guid>(), keyword, selectedLevels);

            //Assert
            var clearLinkCount = selectedLevels.Count;
            Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                Assert.IsNotNull(levelViewModel);
                Assert.IsTrue(model.ClearLevelLinks.ContainsKey(levelViewModel.Title));
                Assert.AreEqual(clearLinkCount - 1, model.ClearLevelLinks.Count(c => c.Value.Contains($"levels={selectedLevel}")));
                Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"keyword={keyword}")));
                Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"orderby=")));
            }
        }
        
        [Test, AutoData]
        public void Then_Has_Keyword_And_OrderBy_Then_Builds_QueryString_With_Keyword_And_OrderBy(List<int> selectedLevels, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(new List<Guid>(), keyword, selectedLevels);

            //Assert
            var clearLinkCount = selectedLevels.Count;
            Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                Assert.IsNotNull(levelViewModel);
                Assert.IsTrue(model.ClearLevelLinks.ContainsKey(levelViewModel.Title));
                Assert.AreEqual(clearLinkCount - 1, model.ClearLevelLinks.Count(c => c.Value.Contains($"levels={selectedLevel}")));
                Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"keyword={keyword}")));
                Assert.AreEqual(clearLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"orderby={OrderBy.Name}")));
            }
        }

        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Levels_And_Sectors(List<int> selectedLevels, List<Guid> selectedRoutes, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, selectedLevels);

            //Assert
            var clearSectorsLinkCount = selectedRoutes.Count;
            Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count);
            var clearLevelsLinkCount = selectedLevels.Count;
            Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                Assert.IsNotNull(levelViewModel);
                Assert.IsTrue(model.ClearLevelLinks.ContainsKey(levelViewModel.Title));
                Assert.AreEqual(clearLevelsLinkCount - 1, model.ClearLevelLinks.Count(c => c.Value.Contains($"levels={selectedLevel}")));
                Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"?keyword={keyword}")));
                Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains("&sectors=" + string.Join("&sectors=", model.SelectedSectors))));
                Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"orderby=")));
            }
            foreach (var selectedRoute in selectedRoutes)
            {
                var sector = model.Sectors.SingleOrDefault(c => c.Id.Equals(selectedRoute));

                Assert.IsNotNull(sector);
                Assert.IsTrue(model.ClearSectorLinks.ContainsKey(sector.Route));
                Assert.AreEqual(clearSectorsLinkCount - 1, model.ClearSectorLinks.Count(c => c.Value.Contains($"sectors={selectedRoute}")));
                Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count(c => c.Value.Contains($"?keyword={keyword}")));
                Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count(c => c.Value.Contains("&levels=" + string.Join("&levels=", model.SelectedLevels))));
                Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count(c => c.Value.Contains($"orderby=")));
            }
        }
        [Test, AutoData]
        public void Then_The_Clear_Filter_Items_Are_Built_From_The_Selected_Levels_And_Sectors_And_OrderBy(List<int> selectedLevels, List<Guid> selectedRoutes, string keyword)
        {
            //Arrange Act
            var model = CoursesViewModelFactory.BuildModel(selectedRoutes, keyword, selectedLevels);

            //Assert
            var clearSectorsLinkCount = selectedRoutes.Count;
            Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count);
            var clearLevelsLinkCount = selectedLevels.Count;
            Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count);

            foreach (var selectedLevel in selectedLevels)
            {
                var levelViewModel = model.Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));

                Assert.IsNotNull(levelViewModel);
                Assert.IsTrue(model.ClearLevelLinks.ContainsKey(levelViewModel.Title));
                Assert.AreEqual(clearLevelsLinkCount - 1, model.ClearLevelLinks.Count(c => c.Value.Contains($"levels={selectedLevel}")));
                Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"?keyword={keyword}")));
                Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains($"&orderby={OrderBy.Name}")));
                Assert.AreEqual(clearLevelsLinkCount, model.ClearLevelLinks.Count(c => c.Value.Contains("&sectors=" + string.Join("&sectors=", model.SelectedSectors))));
            }
            foreach (var selectedRoute in selectedRoutes)
            {
                var sector = model.Sectors.SingleOrDefault(c => c.Id.Equals(selectedRoute));

                Assert.IsNotNull(sector);
                Assert.IsTrue(model.ClearSectorLinks.ContainsKey(sector.Route));
                Assert.AreEqual(clearSectorsLinkCount - 1, model.ClearSectorLinks.Count(c => c.Value.Contains($"sectors={selectedRoute}")));
                Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count(c => c.Value.Contains($"?keyword={keyword}")));
                Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count(c => c.Value.Contains($"&orderby={OrderBy.Name}")));
                Assert.AreEqual(clearSectorsLinkCount, model.ClearSectorLinks.Count(c => c.Value.Contains("&levels=" + string.Join("&levels=", model.SelectedLevels))));
            }
        }
    }
}