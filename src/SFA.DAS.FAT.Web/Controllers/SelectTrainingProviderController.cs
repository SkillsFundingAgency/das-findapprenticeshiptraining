using System.Threading;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("selectProvider", Name = RouteNames.SelectProvider)]
public class SelectTrainingProviderController(FluentValidation.IValidator<SelectTrainingProviderSubmitViewModel> _validator) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        SelectTrainingProviderViewModel model = new SelectTrainingProviderViewModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SelectTrainingProviderSubmitViewModel submitModel, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            var model = new SelectTrainingProviderViewModel();
            result.AddToModelState(ModelState);
            return View(model);
        }

        return RedirectToRoute(RouteNames.Provider, new
        {
            submitModel.Ukprn,
            submitModel.Location
        });
    }
}
