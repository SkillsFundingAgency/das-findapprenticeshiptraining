using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("selectProvider", Name = RouteNames.SelectProvider)]
public class SelectTrainingProviderController(FluentValidation.IValidator<SelectTrainingProviderSubmitViewModel> _validator, ICookieStorageService<LocationCookieItem> locationCookieService) : Controller
{
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieService = locationCookieService;

    [HttpGet]
    public IActionResult Index()
    {
        var locationCookieItem = _locationCookieService.Get(Constants.LocationCookieName);
        SelectTrainingProviderViewModel model = new SelectTrainingProviderViewModel();
        model.Location = locationCookieItem?.Location;
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SelectTrainingProviderSubmitViewModel submitModel, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            var model = new SelectTrainingProviderViewModel();
            ModelState.AddValidationErrors(result.Errors);
            return View(model);
        }
        _locationCookieService.Update(Constants.LocationCookieName, new LocationCookieItem { Location = submitModel.Location, Distance = DistanceService.TenMiles.ToString() });


        return RedirectToRoute(RouteNames.Provider, new
        {
            submitModel.Ukprn,
            submitModel.Location
        });
    }
}
