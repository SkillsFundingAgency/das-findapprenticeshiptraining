using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Domain
{
    public class GetLocationsApiRequest : IGetApiRequest
    {
        private readonly string _searchTerm;

        public GetLocationsApiRequest(string baseUrl, string searchTerm)
        {
            BaseUrl = baseUrl;
            _searchTerm = searchTerm;
        }

        public string BaseUrl { get; }
        public string GetUrl => $"{BaseUrl}locations?searchTerm={_searchTerm}";
    }
}
