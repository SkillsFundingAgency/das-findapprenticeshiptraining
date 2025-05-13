using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.FAT.Application.Locations.Queries.GetLocations;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Controllers
{
    [Route("[controller]")]
    public class LocationsController : Controller
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly IMediator _mediator;

        public LocationsController(
            ILogger<CoursesController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Locations([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Trim().Length < 3)
            {
                return new JsonResult(new LocationsViewModel { Locations = new List<LocationViewModel>() });
            }

            var result = await _mediator.Send(new GetLocationsQuery
            {
                SearchTerm = searchTerm
            });

            var model = new LocationsViewModel
            {
                Locations = result.LocationItems.Select(c => (LocationViewModel)c).ToList()
            };

            return new JsonResult(model);
        }
    }
}
