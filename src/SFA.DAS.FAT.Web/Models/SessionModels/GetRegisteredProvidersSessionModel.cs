using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Providers;

namespace SFA.DAS.FAT.Web.Models.SessionModels;

public class GetRegisteredProvidersSessionModel
{
    public List<RegisteredProvider> Providers { get; set; }
}
