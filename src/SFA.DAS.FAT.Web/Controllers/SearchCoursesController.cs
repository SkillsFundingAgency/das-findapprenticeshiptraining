using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("search", Name = RouteNames.SearchCourses)]
public class SearchCoursesController() : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        SearchCoursesViewModel model = new SearchCoursesViewModel
        {
            ShowSearchCrumb = false,
            ShowShortListLink = true,
        };

        model.TrainingTypesFilterItems = ApprenticeshipTypesFilterHelper.BuildItems(model.SelectedTypes);
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SearchCoursesViewModel submitModel, CancellationToken cancellationToken)
    {
        var request = new GetCoursesViewModel();

        if (!string.IsNullOrEmpty(submitModel.CourseTerm))
        {
            request.Keyword = submitModel.CourseTerm;
        }

        if (!string.IsNullOrEmpty(submitModel.Location))
        {
            request.Location = submitModel.Location;
            request.Distance = DistanceService.TEN_MILES.ToString();
        }
        request.ApprenticeshipTypes = submitModel.SelectedTypes;

        return RedirectToAction("Index", "Courses", request);
    }
}
