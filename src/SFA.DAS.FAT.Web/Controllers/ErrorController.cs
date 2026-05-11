using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Web.Infrastructure;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("error")]
public class ErrorController : Controller
{
    [HttpGet]
    [Route("404", Name = RouteNames.Error404)]
    public IActionResult PageNotFound()
    {
        return View();
    }

    [HttpGet]
    [Route("500", Name = RouteNames.Error500)]
    public IActionResult ApplicationError()
    {
        return View();
    }
}
