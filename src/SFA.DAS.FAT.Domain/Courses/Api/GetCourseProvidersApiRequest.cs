using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api
{
    public class GetCourseProvidersApiRequest : IGetApiRequest
    {
        private readonly string _location;
        private readonly IEnumerable<ProviderDeliveryMode> _deliveryModeTypes;
        private readonly IEnumerable<ProviderRating> _employerProviderRatingTypes;
        private readonly IEnumerable<ProviderRating> _apprenticeProviderRatingTypes;
        private readonly IEnumerable<QarRating> _qarRatings;

        private readonly ProviderOrderBy _orderBy;
        private readonly int? _distance;
        private readonly int? _page;
        private readonly int? _pageSize;
        private readonly Guid? _shortlistUserId;
        private readonly int _id;

        public GetCourseProvidersApiRequest(string baseUrl, int id, ProviderOrderBy orderBy, int? distance, string location,
            IEnumerable<ProviderDeliveryMode> deliveryModeTypes, IEnumerable<ProviderRating> employerProviderRatingTypes,
            IEnumerable<ProviderRating> apprenticeProviderRatingTypes, IEnumerable<QarRating> qarRatings,
            int? page, int? pageSize, Guid? shortlistUserId = null)
        {
            BaseUrl = baseUrl;
            _id = id;
            _orderBy = orderBy;
            _distance = distance;
            _location = location;
            _deliveryModeTypes = deliveryModeTypes;
            _employerProviderRatingTypes = employerProviderRatingTypes;
            _apprenticeProviderRatingTypes = apprenticeProviderRatingTypes;
            _qarRatings = qarRatings;
            _page = page;
            _pageSize = pageSize;
            _shortlistUserId = shortlistUserId;
        }

        public string BaseUrl { get; }
        public string GetUrl => BuildUrl();

        private string BuildUrl()
        {
            var buildUrl = $"{BaseUrl}courses/{_id}/providers?location={HttpUtility.UrlEncode(_location)}&orderBy={_orderBy}";

            if (_distance != 0)
            {
                buildUrl += $"&distance={_distance}";
            }

            if (string.IsNullOrEmpty(_location))
            {
                buildUrl += $"&location={_location}";
            }

            if (_deliveryModeTypes != null && _deliveryModeTypes.Any())
            {
                buildUrl += $"&deliveryModes={string.Join("&deliveryModes=", _deliveryModeTypes)}";
            }
            if (_employerProviderRatingTypes != null && _employerProviderRatingTypes.Any())
            {
                buildUrl += $"&employerProviderRatings={string.Join("&employerProviderRatings=", _employerProviderRatingTypes)}";
            }
            if (_apprenticeProviderRatingTypes != null && _apprenticeProviderRatingTypes.Any())
            {
                buildUrl += $"&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", _apprenticeProviderRatingTypes)}";
            }

            if (_qarRatings != null && _qarRatings.Any())
            {
                buildUrl += $"&qar={string.Join("&qar=", _qarRatings)}";
            }

            if (_page != null)
            {
                buildUrl += $"&page={_page}";
            }
            if (_pageSize != 0)
            {
                buildUrl += $"&pageSize={_pageSize}";
            }

            if (_shortlistUserId != null)
            {
                buildUrl += $"&shortlistUserId={_shortlistUserId}";
            }

            return buildUrl;
        }
    }
}
