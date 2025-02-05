using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain.Courses.Api.Requests
{
    public class GetCoursesApiRequest : IGetApiRequest
    {
        private readonly Guid? _shortlistUserId;

        public GetCoursesApiRequest(string baseUrl, string keyword, List<string> sectors = null, List<int> levels = null, OrderBy orderBy = OrderBy.None, Guid? shortlistUserId = null)
        {
            _shortlistUserId = shortlistUserId;
            BaseUrl = baseUrl;
            Keyword = keyword;
            Sectors = sectors;
            Levels = levels;
            OrderBy = orderBy;
        }

        public List<string> Sectors { get; }
        public List<int> Levels { get; }

        public string BaseUrl { get; }
        public string GetUrl => BuildUrl();

        private string BuildUrl()
        {
            var url = $"{BaseUrl}trainingcourses?keyword={Keyword}";
            if (OrderBy != OrderBy.None)
            {
                url += $"&orderby={OrderBy}";
            }
            if (Sectors != null && Sectors.Any())
            {
                url += "&routeIds=" + string.Join("&routeIds=", Sectors.Select(HttpUtility.HtmlEncode));
            }
            if (Levels != null && Levels.Any())
            {
                url += "&levels=" + string.Join("&levels=", Levels);
            }

            if (_shortlistUserId.HasValue)
            {
                url += $"&shortlistUserId={_shortlistUserId}";
            }
            return url;
        }

        public string Keyword { get; }
        public OrderBy OrderBy { get; }
    }

}

