using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("selectProvider", Name = RouteNames.SelectProvider)]
public class SelectTrainingProviderController(ICookieStorageService<ShortlistCookieItem> shortlistCookieService, IShortlistService shortlistService, FluentValidation.IValidator<SelectTrainingProviderSubmitViewModel> _validator) : Controller
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

        SelectTrainingProviderViewModel model = new SelectTrainingProviderViewModel(shortListItemCount);
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SelectTrainingProviderSubmitViewModel submitModel, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            var model = new SelectTrainingProviderViewModel(submitModel.ShortListItemCount);
            result.AddToModelState(ModelState);
            return View(model);
        }

        return RedirectToAction("Index", "SelectTrainingProvider");
    }
}
