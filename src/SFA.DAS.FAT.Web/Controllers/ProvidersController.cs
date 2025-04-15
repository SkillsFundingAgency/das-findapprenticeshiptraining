using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Application.Providers.Query.GetProvider;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("providers/{ukprn}", Name = RouteNames.Provider)]
public class ProvidersController : Controller
{
    private readonly IMediator _mediator;

    public ProvidersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromQuery] string location)
    {
        var response = await _mediator.Send(new GetProviderQuery(ukprn));

        var vm = (ProviderDetailsViewModel)response;
        vm.Location = location;
        return View(vm);
    }
}

