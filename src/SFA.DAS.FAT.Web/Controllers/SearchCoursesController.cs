using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("search", Name = RouteNames.SearchCourses)]
public class SearchCoursesController : Controller
{
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieService;

    public SearchCoursesController(ICookieStorageService<LocationCookieItem> locationCookieService)
    {
        _locationCookieService = locationCookieService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        SearchCoursesViewModel model = new SearchCoursesViewModel
        {
            ShowSearchCrumb = false,
            ShowShortListLink = true,
            TrainingTypesFilterItems = LearningTypesFilterHelper.BuildItems([], true)
        };
        _locationCookieService.Delete(Constants.LocationCookieName);
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SearchCoursesViewModel submitModel)
    {
        var request = new GetCoursesViewModel
        {
            LearningTypes = submitModel.SelectedTypes
        };

        if (!string.IsNullOrEmpty(submitModel.CourseTerm))
        {
            request.Keyword = submitModel.CourseTerm;
        }

        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location?.Trim(), Distance = DistanceService.TenMiles.ToString() });
        return RedirectToRoute(RouteNames.Courses, request);
    }
}
