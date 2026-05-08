using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Web.Extensions;
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
            ModelState.AddValidationErrors(result.Errors);
            return View(model);
        }

        return RedirectToRoute(RouteNames.Provider, new
        {
            submitModel.Ukprn,
            submitModel.Location
        });
    }
}
