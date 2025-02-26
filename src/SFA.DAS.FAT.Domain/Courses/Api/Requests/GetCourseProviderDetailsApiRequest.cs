using System;
using System.Web;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests
{
    public class GetCourseProviderDetailsApiRequest : IGetApiRequest
    {
        private readonly int _providerId;
        private readonly string _location;
        private readonly Guid _shortlistUserId;
        private readonly double _lat;
        private readonly double _lon;

        private readonly int _courseId;

        public GetCourseProviderDetailsApiRequest(string baseUrl, int courseId, int providerId, string location, Guid shortlistUserId, double lat = 0, double lon = 0)
        {
            _providerId = providerId;
            _location = location;
            _shortlistUserId = shortlistUserId;
            _lat = lat;
            _lon = lon;
            BaseUrl = baseUrl;
            _courseId = courseId;
        }

        public string BaseUrl { get; }
        public string GetUrl => $"{BaseUrl}trainingcourses/{_courseId}/providers/{_providerId}?location={HttpUtility.UrlEncode(_location)}&lat={_lat}&lon={_lon}&shortlistUserId={_shortlistUserId}";
    }
}
