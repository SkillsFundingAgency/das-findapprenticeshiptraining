using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("error")]
public class ErrorController : Controller
{
    public const string PageNotFoundViewName = "PageNotFound";
    public const string ErrorInServiceViewName = "ErrorInService";
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("{statuscode}")]
    public IActionResult HttpStatusCodeHandler([FromRoute] int statusCode)
    {
        _logger.LogError("Unexpected error occurred with status code: {StatusCode}", statusCode);

        return statusCode switch
        {
            StatusCodes.Status404NotFound => View(PageNotFoundViewName),
            _ => View(ErrorInServiceViewName)
        };
    }

    public IActionResult ErrorInService()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        _logger.LogError(feature.Error, "Unexpected error occurred during request to {Path}", feature.Path);

        return View();
    }
}
