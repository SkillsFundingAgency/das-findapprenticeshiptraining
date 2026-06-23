using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Application.Providers.Query.GetProvider;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;
using SFA.DAS.FAT.Web.Models.Providers;

namespace SFA.DAS.FAT.Web.Controllers;

[Route("providers/{ukprn}", Name = RouteNames.Provider)]
public class ProvidersController : Controller
{
    private readonly IMediator _mediator;
    private readonly IValidator<GetCourseProviderDetailsQuery> _ukprnValidator;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICookieStorageService<LocationCookieItem> _locationCookieService;

    public ProvidersController(IMediator mediator, IDateTimeService dateTimeService, IValidator<GetCourseProviderDetailsQuery> ukprnValidator, ICookieStorageService<LocationCookieItem> locationCookieService)
    {
        _mediator = mediator;
        _dateTimeService = dateTimeService;
        _ukprnValidator = ukprnValidator;
        _locationCookieService = locationCookieService;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index([FromRoute] int ukprn)
    {
        var validationResult = await _ukprnValidator.ValidateAsync(new GetCourseProviderDetailsQuery { Ukprn = ukprn });

        if (!validationResult.IsValid)
        {
            return NotFound();
        }

        var response = await _mediator.Send(new GetProviderQuery(ukprn));

        if (response == null)
        {
            return NotFound();
        }

        var viewModel = (ProviderDetailsViewModel)response;
        viewModel.FeedbackSurvey = FeedbackSurveyViewModel.ProcessFeedbackDetails(response.AnnualEmployerFeedbackDetails,
            response.AnnualApprenticeFeedbackDetails, _dateTimeService.GetDateTime());
        var locationCookieItem = _locationCookieService.Get(Constants.LocationCookieName);

        viewModel.Location = locationCookieItem?.Location;
        return View(viewModel);
    }
}

