using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Application.Providers.Queries.GetRegisteredProviders;
using SFA.DAS.FAT.Web.Infrastructure;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("registeredProviders", Name = RouteNames.GetRegisteredProviders)]
public class RegisteredProvidersController(IMediator _mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetRegisteredProviders([FromQuery] string query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRegisteredProvidersQuery { Query = query }, cancellationToken);
        return Ok(result.Providers.Take(100));
    }
}
