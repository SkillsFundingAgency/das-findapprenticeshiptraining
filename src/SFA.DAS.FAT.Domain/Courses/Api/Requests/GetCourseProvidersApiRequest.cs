using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests
{
    public class GetCourseProvidersApiRequest : IGetApiRequest
    {
        private readonly string _location;
        private readonly IEnumerable<DeliveryModeType> _deliveryModeTypes;
        private readonly IEnumerable<ProviderRating> _employerProviderRatingTypes;
        private readonly IEnumerable<ProviderRating> _apprenticeProviderRatingTypes;

        private readonly int _sortOrder;
        private readonly double _lat;
        private readonly double _lon;
        private readonly Guid? _shortlistUserId;
        private readonly int _id;

        public GetCourseProvidersApiRequest(string baseUrl, int id, string location,
            IEnumerable<DeliveryModeType> deliveryModeTypes, IEnumerable<ProviderRating> employerProviderRatingTypes,
            IEnumerable<ProviderRating> apprenticeProviderRatingTypes, int sortOrder = 0, double lat = 0, double lon = 0, Guid? shortlistUserId = null)
        {
            _location = location;
            _deliveryModeTypes = deliveryModeTypes;
            _sortOrder = sortOrder;
            _lat = lat;
            _lon = lon;
            _shortlistUserId = shortlistUserId;
            BaseUrl = baseUrl;
            _id = id;
            _employerProviderRatingTypes = employerProviderRatingTypes;
            _apprenticeProviderRatingTypes = apprenticeProviderRatingTypes;
        }

        public string BaseUrl { get; }
        public string GetUrl => BuildUrl();

        private string BuildUrl()
        {
            var buildUrl = $"{BaseUrl}trainingcourses/{_id}/providers?location={HttpUtility.UrlEncode(_location)}&sortOrder={_sortOrder}";
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

            if (_lat != 0)
            {
                buildUrl += $"&lat={_lat}";
            }
            if (_lon != 0)
            {
                buildUrl += $"&lon={_lon}";
            }

            if (_shortlistUserId != null)
            {
                buildUrl += $"&shortlistUserId={_shortlistUserId}";
            }

            return buildUrl;
        }
    }
}
