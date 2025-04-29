using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class GetProviderAddress
{
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }

    public static implicit operator GetProviderAddress(GetProviderAddressModel source)
    {
        if (source == null) return new GetProviderAddress();

        return new GetProviderAddress
        {
            AddressLine1 = source.AddressLine1,
            AddressLine2 = source.AddressLine2,
            AddressLine3 = source.AddressLine3,
            AddressLine4 = source.AddressLine4,
            Town = source.Town,
            Postcode = source.Postcode
        };
    }

    public string GetComposedAddress(string providerName)
    {

        var addressItems = new List<string>
        {
            providerName,
            AddressLine1,
            AddressLine2,
            AddressLine3,
            AddressLine4,
            Town,
            Postcode
        };

        return string.Join(", ", addressItems.Where(s => !string.IsNullOrEmpty(s)).ToArray());

    }
}
