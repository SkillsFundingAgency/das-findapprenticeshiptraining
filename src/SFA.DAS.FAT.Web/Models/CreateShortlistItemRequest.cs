namespace SFA.DAS.FAT.Web.Models;

public class CreateShortlistItemRequest
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string RouteName { get; set; }
    public string ProviderName { get; set; }
    public string LocationName { get; set; } = null;
}
