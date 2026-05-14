using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Web.Infrastructure;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("[controller]")]
public class PrivacyController : Controller
{
    [HttpGet]
    [Route("", Name = RouteNames.Privacy)]
    public IActionResult Privacy()
    {
        return View();
    }
}
