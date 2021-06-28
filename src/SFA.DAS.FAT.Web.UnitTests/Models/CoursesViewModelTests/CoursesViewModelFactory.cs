using AutoFixture;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests
{
    public static class CoursesViewModelFactory
    {
        public static CoursesViewModel BuildModel(List<string> selectedRoutes, string keyword, List<int> selectedLevels, OrderBy orderBy = OrderBy.Name)
        {
            var fixture = new Fixture();
            var sectors = selectedRoutes
                .Select(selectedRoute => new SectorViewModel(
                    new Sector
                    {
                        Route = selectedRoute
                    }, null))
                .ToList();
            var levels = selectedLevels
                .Select(selectedLevel => new LevelViewModel(
                    new Level
                    {
                        Code = selectedLevel,
                        Name = fixture.Create<string>()
                    }, null))
                .ToList();

            var model = new CoursesViewModel
            {
                Sectors = sectors,
                Levels = levels,
                Keyword = keyword,
                SelectedSectors = selectedRoutes,
                SelectedLevels = selectedLevels,
                OrderBy = orderBy
            };
            return model;
        }
    }
}
