using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Web.Infrastructure;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Web.Controllers
{
    [Route("[controller]")]
    public class CoursesController : Controller
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly IMediator _mediator;
        private readonly ICookieStorageService<LocationCookieItem> _locationCookieStorageService;
        private readonly ICookieStorageService<GetCourseProvidersRequest> _courseProvidersCookieStorageService;
        private readonly ICookieStorageService<ShortlistCookieItem> _shortlistCookieService;
        private readonly FindApprenticeshipTrainingWeb _config;
        private readonly IDataProtector _providerDataProtector;
        private readonly IDataProtector _shortlistDataProtector;

        public CoursesController (
            ILogger<CoursesController> logger,
            IMediator mediator,
            ICookieStorageService<LocationCookieItem> locationCookieStorageService,
            ICookieStorageService<GetCourseProvidersRequest> courseProvidersCookieStorageService,
            ICookieStorageService<ShortlistCookieItem> shortlistCookieService,
            IDataProtectionProvider provider,
            IOptions<FindApprenticeshipTrainingWeb> config)
        {
            _logger = logger;
            _mediator = mediator;
            _locationCookieStorageService = locationCookieStorageService;
            _courseProvidersCookieStorageService = courseProvidersCookieStorageService;
            _shortlistCookieService = shortlistCookieService;
            _config = config.Value;
            _providerDataProtector = provider.CreateProtector(Constants.GaDataProtectorName);
            _shortlistDataProtector = provider.CreateProtector(Constants.ShortlistProtectorName);
        }

        [Route("", Name = RouteNames.Courses)]
        public async Task<IActionResult> Courses(GetCoursesRequest request)
        {
            var location = CheckLocation(request.Location);
            var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
            
            var result = await _mediator.Send(new GetCoursesQuery
            {
                Keyword = request.Keyword,
                RouteIds = request.Sectors,
                Levels = request.Levels,
                OrderBy = request.OrderBy,
                ShortlistUserId = shortlistItem?.ShortlistUserId
            });

            var viewModel = new CoursesViewModel
            {
                Courses = result.Courses.Select(c => (CourseViewModel)c).ToList(),
                Sectors = result.Sectors.Select(sector => new SectorViewModel(sector, request.Sectors)).ToList(),
                Total = result.Total,
                TotalFiltered = result.TotalFiltered,
                Keyword = request.Keyword,
                SelectedSectors = request.Sectors,
                SelectedLevels = request.Levels,
                Levels = result.Levels.Select(level => new LevelViewModel(level, request.Levels)).ToList(),
                OrderBy = request.OrderBy,
                ShortlistItemCount = result.ShortlistItemCount,
                Location = location?.Name ?? ""
            };

            return View(viewModel);
        }

        [Route("{id}", Name = RouteNames.CourseDetails)]
        public async Task<IActionResult> CourseDetail(int id, [FromQuery(Name="location")]string locationName)
        {
            var location = CheckLocation(locationName);
            var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
            
            var result = await _mediator.Send(new GetCourseQuery
            {
                CourseId = id,
                Lat = location?.Lat ?? 0,
                Lon = location?.Lon ?? 0,
                ShortlistUserId = shortlistItem?.ShortlistUserId
            });

            if (result.Course == null)
            {
                return RedirectToRoute(RouteNames.Error404);
            }
            
            var viewModel = (CourseViewModel)result.Course;
            viewModel.LocationName = location?.Name;
            viewModel.TotalProvidersCount = result.ProvidersCount?.TotalProviders;
            viewModel.ProvidersAtLocationCount = result.ProvidersCount?.ProvidersAtLocation;
            viewModel.ShortlistItemCount = result.ShortlistItemCount;
            viewModel.HelpFindingCourseUrl = BuildHelpFindingCourseUrl(viewModel.Id, result.ShowEmployerDemand);
            
            return View(viewModel);
        }

        

        [Route("{id}/providers", Name = RouteNames.CourseProviders)]
        public async Task<IActionResult> CourseProviders(GetCourseProvidersRequest request)
        {
            try
            {
                var location = CheckLocation(request.Location);

                var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
                
                var result = await _mediator.Send(new GetCourseProvidersQuery
                {
                    CourseId = request.Id,
                    Location = location?.Name ?? "",
                    Lat = location?.Lat ?? 0,
                    Lon = location?.Lon ?? 0,
                    DeliveryModes = request.DeliveryModes.Select(type => (Domain.Courses.DeliveryModeType)type),
                    ProviderRatings = request.ProviderRatings.Select(rating => (Domain.Courses.ProviderRating)rating),
                    ShortlistUserId = shortlistItem?.ShortlistUserId
                });
                
                var cookieResult =new LocationCookieItem
                {
                    Name = result.Location,
                    Lat = result.LocationGeoPoint?.FirstOrDefault() ?? 0,
                    Lon = result.LocationGeoPoint?.LastOrDefault() ?? 0
                }; 
                UpdateLocationCookie(cookieResult);
                
                if (result.Course == null)
                {
                    return RedirectToRoute(RouteNames.Error404);
                }

                var providerList = result.Providers.ToList();

                var providers = result.Providers
                        .ToDictionary(provider => 
                                        provider.ProviderId, 
                                        provider => WebEncoders.Base64UrlEncode(_providerDataProtector.Protect(
                                            System.Text.Encoding.UTF8.GetBytes($"{providerList.IndexOf(provider) + 1}|{result.TotalFiltered}"))));
                

                _courseProvidersCookieStorageService.Update(Constants.ProvidersCookieName, request, 2);

                var courseProvidersViewModel = new CourseProvidersViewModel(request, result, providers);

                if (courseProvidersViewModel.Course.AfterLastStartDate)
                {
                    return RedirectToRoute(RouteNames.CourseDetails,new {request.Id});
                }

                var removedProviderFromShortlist =
                    string.IsNullOrEmpty(request.Removed) ? "" : HttpUtility.HtmlDecode(GetEncodedProviderName(request.Removed));
                var addedProviderToShortlist =
                    string.IsNullOrEmpty(request.Added) ? "" : HttpUtility.HtmlDecode(GetEncodedProviderName(request.Added));
                
                courseProvidersViewModel.BannerUpdateMessage = GetProvidersBannerUpdateMessage(removedProviderFromShortlist, addedProviderToShortlist);
                
                courseProvidersViewModel.HelpFindingCourseUrl = BuildHelpFindingCourseUrl(courseProvidersViewModel.Course.Id, result.ShowEmployerDemand);
                
                return View(courseProvidersViewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToRoute(RouteNames.Error500);
            }
        }

        [Route("{id}/providers/{providerId}", Name = RouteNames.CourseProviderDetails)]
        public async Task<IActionResult> CourseProviderDetail(int id, int providerId, string location, string removed, string added)
        {
            try
            {   
                var locationItem = CheckLocation(location);
                var shortlistItem = _shortlistCookieService.Get(Constants.ShortlistCookieName);
                
                var result = await _mediator.Send(new GetCourseProviderQuery
                {
                    ProviderId = providerId ,
                    CourseId = id, 
                    Location = locationItem?.Name ?? "",
                    Lat = locationItem?.Lat ?? 0,
                    Lon = locationItem?.Lon ?? 0,
                    ShortlistUserId = shortlistItem?.ShortlistUserId
                });

                var cookieResult =new LocationCookieItem
                {
                    Name = result.Location,
                    Lat = result.LocationGeoPoint?.FirstOrDefault() ?? 0,
                    Lon = result.LocationGeoPoint?.LastOrDefault() ?? 0
                }; 
                UpdateLocationCookie(cookieResult);

                if (result.Course == null)
                {
                    return RedirectToRoute(RouteNames.Error404);
                }
                
                var viewModel = (CourseProviderViewModel)result;
                viewModel.Location = cookieResult.Name;
                var providersRequestCookie = _courseProvidersCookieStorageService.Get(Constants.ProvidersCookieName);
                if (providersRequestCookie != default)
                {
                    if (id != providersRequestCookie.Id)
                    {
                        providersRequestCookie.Id = id;
                        providersRequestCookie.DeliveryModes = new List<DeliveryModeType>();
                        providersRequestCookie.ProviderRatings = new List<ProviderRating>();
                    }
                    
                    providersRequestCookie.Location = result?.Location;
                    viewModel.GetCourseProvidersRequest = providersRequestCookie.ToDictionary();
                }

                if (viewModel.Course.AfterLastStartDate)
                {
                    return RedirectToRoute(RouteNames.CourseDetails,new {Id = id});
                }

                var removedProviderFromShortlist =
                    string.IsNullOrEmpty(removed) ? "" : HttpUtility.HtmlDecode(GetEncodedProviderName(removed));
                var addedProviderToShortlist =
                    string.IsNullOrEmpty(added) ? "" : HttpUtility.HtmlDecode(GetEncodedProviderName(added));
                
                viewModel.BannerUpdateMessage = GetProvidersBannerUpdateMessage(removedProviderFromShortlist, addedProviderToShortlist);
                
                viewModel.HelpFindingCourseUrl = BuildHelpFindingCourseUrl(viewModel.Course.Id, result.ShowEmployerDemand);
                
                return View(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return RedirectToRoute(RouteNames.Error500);
            }
        }

        private LocationCookieItem CheckLocation(string location)
        {
            if (location == "-1")
            {
                _locationCookieStorageService.Delete(Constants.LocationCookieName);
                return null;
            }
            if (string.IsNullOrEmpty(location))
            {
                return _locationCookieStorageService.Get(Constants.LocationCookieName);
            }

            return new LocationCookieItem
            {
                Name = location
            };
        }

        private void UpdateLocationCookie(LocationCookieItem location)
        {
            if (!string.IsNullOrEmpty(location.Name) && location.Lat != 0 && location.Lon != 0)
            {
                _locationCookieStorageService.Update(Constants.LocationCookieName, location, 2);    
            }
        }

        private string GetEncodedProviderName(string encodedName)
        {
            try
            {
                var base64EncodedBytes = WebEncoders.Base64UrlDecode(encodedName);
                return System.Text.Encoding.UTF8.GetString(_shortlistDataProtector.Unprotect(base64EncodedBytes));
            }
            catch (FormatException e)
            {
                _logger.LogInformation(e,"Unable to decode provider name from request");
            }
            catch (CryptographicException e)
            {
                _logger.LogInformation(e, "Unable to decode provider name from request");
            }

            return string.Empty;
        }

        private static string GetProvidersBannerUpdateMessage(string removedProviderFromShortlist, string addedProviderToShortlist)
        {
            if (!string.IsNullOrEmpty(removedProviderFromShortlist))
            {
                return $"{removedProviderFromShortlist} removed from shortlist.";
            } 
            
            if (!string.IsNullOrEmpty(addedProviderToShortlist))
            {
                return $"{addedProviderToShortlist} added to shortlist.";
            }

            return "";
        }
        
        private string BuildHelpFindingCourseUrl(int courseId, bool showDemand)
        {
            return _config.EmployerDemandFeatureToggle && showDemand? 
                $"{_config.EmployerDemandUrl}/registerdemand/course/{courseId}/enter-apprenticeship-details" 
                : "https://help.apprenticeships.education.gov.uk/hc/en-gb#contact-us";
        }
    }
}
