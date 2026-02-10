using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
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
            ShowShortListLink = true,
            Types = new List<TypeViewModel>()
        };

        var types = new List<ApprenticeType>
        {
            new()
            {
                Code = ApprenticeshipType.ApprenticeshipUnit.ToString(),
                Name = ApprenticeshipType.ApprenticeshipUnit.GetDescription()
            },
            new()
            {
                Code = ApprenticeshipType.FoundationApprenticeship.ToString(),
                Name = ApprenticeshipType.FoundationApprenticeship.GetDescription()
            },
            new()
            {
                Code = ApprenticeshipType.Apprenticeship.ToString(),
                Name= ApprenticeshipType.Apprenticeship.GetDescription()
            }
        };

        model.Types = types.Select(type => new TypeViewModel(type, [])).ToList();

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
