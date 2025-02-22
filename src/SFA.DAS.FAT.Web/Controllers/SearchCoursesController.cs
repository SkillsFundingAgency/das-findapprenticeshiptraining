using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("search", Name = RouteNames.SearchCourses)]
public class SearchCoursesController(ICookieStorageService<ShortlistCookieItem> shortlistCookieService, IShortlistService shortlistService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var shortListItemCount = 0;

        var shortlistItem = shortlistCookieService.Get(Constants.ShortlistCookieName);
        var shortlistUserId = shortlistItem?.ShortlistUserId;

        if (shortlistUserId != null)
        {
            ShortlistForUser shortlist = await shortlistService.GetShortlistForUser((Guid)shortlistUserId);
            shortListItemCount = shortlist.Shortlist.Count();
        }

        SearchCoursesViewModel model = new SearchCoursesViewModel
        {
            ShortListItemCount = shortListItemCount,
            ShowBackLink = true,
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
            request.Distance = Constants.DefaultDistance.ToString();
        }

        return RedirectToAction("Index", "Courses", request);
    }
}
