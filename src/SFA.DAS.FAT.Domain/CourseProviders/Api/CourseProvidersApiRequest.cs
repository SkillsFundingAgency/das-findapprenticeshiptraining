using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.CourseProviders.Api;

public class CourseProvidersApiRequest : IGetApiRequest
{
    private readonly string _location;
    private readonly IEnumerable<ProviderDeliveryMode> _deliveryModeTypes;
    private readonly IEnumerable<EmployerProviderRating> _employerProviderRatingTypes;
    private readonly IEnumerable<ApprenticeProviderRating> _apprenticeProviderRatingTypes;
    private readonly IEnumerable<QarRating> _qarRatings;

    private readonly ProviderOrderBy _orderBy;
    private readonly int? _distance;
    private readonly int? _page;
    private readonly int? _pageSize;
    private readonly Guid? _shortlistUserId;
    private readonly int _id;

    public CourseProvidersApiRequest(string baseUrl, CourseProvidersParameters courseProvidersParameters)
    {
        BaseUrl = baseUrl;
        _id = courseProvidersParameters.Id;
        _orderBy = courseProvidersParameters.OrderBy;
        _distance = courseProvidersParameters.Distance;
        _location = courseProvidersParameters.Location;
        _deliveryModeTypes = courseProvidersParameters.DeliveryModeTypes;
        _employerProviderRatingTypes = courseProvidersParameters.EmployerProviderRatingTypes;
        _apprenticeProviderRatingTypes = courseProvidersParameters.ApprenticeProviderRatingTypes;
        _qarRatings = courseProvidersParameters.QarRatings;
        _page = courseProvidersParameters.Page;
        _pageSize = courseProvidersParameters.PageSize;
        _shortlistUserId = courseProvidersParameters.ShortlistUserId;
    }

    public string BaseUrl { get; }
    public string GetUrl => BuildUrl();

    private string BuildUrl()
    {
        var buildUrl = $"{BaseUrl}courses/{_id}/providers?orderBy={_orderBy}";

        buildUrl = AddDistanceToUrl(buildUrl);
        buildUrl = AddLocationToUrl(buildUrl);
        buildUrl = AddDeliveryModesToUrl(buildUrl);
        buildUrl = AddProviderRatingsToUrl(buildUrl);
        buildUrl = AddQarRatingsToUrl(buildUrl);
        buildUrl = AddPageToUrl(buildUrl);
        buildUrl = AddPageSizeToUrl(buildUrl);
        buildUrl = AddShortlistUserIdToUrl(buildUrl);

        return buildUrl;
    }

    private string AddShortlistUserIdToUrl(string buildUrl)
    {
        if (_shortlistUserId != null)
        {
            buildUrl += $"&shortlistUserId={_shortlistUserId}";
        }

        return buildUrl;
    }

    private string AddPageSizeToUrl(string buildUrl)
    {
        if (_pageSize != null && _pageSize != 0)
        {
            buildUrl += $"&pageSize={_pageSize}";
        }

        return buildUrl;
    }

    private string AddPageToUrl(string buildUrl)
    {
        if (_page is > 1)
        {
            buildUrl += $"&page={_page}";
        }

        return buildUrl;
    }

    private string AddQarRatingsToUrl(string buildUrl)
    {
        if (_qarRatings != null && _qarRatings.Any())
        {
            buildUrl += $"&qar={string.Join("&qar=", _qarRatings)}";
        }

        return buildUrl;
    }

    private string AddProviderRatingsToUrl(string buildUrl)
    {
        if (_employerProviderRatingTypes != null && _employerProviderRatingTypes.Any())
        {
            buildUrl +=
                $"&employerProviderRatings={string.Join("&employerProviderRatings=", _employerProviderRatingTypes)}";
        }

        if (_apprenticeProviderRatingTypes != null && _apprenticeProviderRatingTypes.Any())
        {
            buildUrl +=
                $"&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", _apprenticeProviderRatingTypes)}";
        }

        return buildUrl;
    }

    private string AddDeliveryModesToUrl(string buildUrl)
    {
        if (_deliveryModeTypes != null && _deliveryModeTypes.Any())
        {
            buildUrl += $"&deliveryModes={string.Join("&deliveryModes=", _deliveryModeTypes)}";
        }

        return buildUrl;
    }

    private string AddLocationToUrl(string buildUrl)
    {
        if (!string.IsNullOrEmpty(_location))
        {
            buildUrl += $"&location={HttpUtility.UrlEncode(_location)}";
        }

        return buildUrl;
    }

    private string AddDistanceToUrl(string buildUrl)
    {
        if (_distance != null && _distance != 0)
        {
            buildUrl += $"&distance={_distance}";
        }

        return buildUrl;
    }
}
