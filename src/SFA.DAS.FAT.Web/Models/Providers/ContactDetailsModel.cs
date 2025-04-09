using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ContactDetailsModel
{
    public string MarketingInfo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public static implicit operator ContactDetailsModel(GetProviderContactDetails source)
    {
        if (source == null) return new ContactDetailsModel();

        var website = source.Website;

        if (website != null && !website.StartsWith("http") && website.Trim() != string.Empty)
        {
            website = $"http://{website}";
        }

        return new ContactDetailsModel
        {
            MarketingInfo = source.MarketingInfo,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Website = website ?? string.Empty
        };
    }
}
