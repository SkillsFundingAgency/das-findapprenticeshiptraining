using System;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests
{
    public class GetCourseApiRequest : IGetApiRequest
    {
        private readonly double _lat;
        private readonly double _lon;
        private readonly string _locationName;
        private readonly Guid? _shortlistUserId;

        public GetCourseApiRequest(string baseUrl, int id, double lat, double lon, string locationName, Guid? shortlistUserId = null)
        {
            _lat = lat;
            _lon = lon;
            _locationName = locationName;
            _shortlistUserId = shortlistUserId;
            BaseUrl = baseUrl;
            Id = id;
        }

        private int Id { get; }
        public string BaseUrl { get; }
        public string GetUrl => $"{BaseUrl}trainingcourses/{Id}?lat={_lat}&lon={_lon}&location={_locationName}&shortlistUserId={_shortlistUserId}";
    }
}