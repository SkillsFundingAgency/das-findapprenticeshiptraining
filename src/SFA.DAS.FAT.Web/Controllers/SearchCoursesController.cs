﻿using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
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
            ShowShortListLink = true
        };

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

        return RedirectToAction("Index", "Courses", request);
    }
}
