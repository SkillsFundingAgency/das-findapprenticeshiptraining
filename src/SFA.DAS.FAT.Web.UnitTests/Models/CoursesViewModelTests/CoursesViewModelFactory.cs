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
            var routes = selectedRoutes
                .Select(selectedRoute => new RouteViewModel(
                    new Route
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
                Routes = sectors,
                Levels = levels,
                Keyword = keyword,
                SelectedRoutes = selectedRoutes,
                SelectedLevels = selectedLevels,
                OrderBy = orderBy
            };
            return model;
        }
    }
}
