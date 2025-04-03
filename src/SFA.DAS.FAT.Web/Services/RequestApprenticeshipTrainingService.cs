using System;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Services;

public interface IRequestApprenticeshipTrainingService
{
    string GetRequestApprenticeshipTrainingUrl(int LarsCode, EntryPoint entryPoint, string location);
}

public class RequestApprenticeshipTrainingService(FindApprenticeshipTrainingWeb config) : IRequestApprenticeshipTrainingService
{
    public string GetRequestApprenticeshipTrainingUrl(int LarsCode, EntryPoint entryPoint, string location)
    {
        string redirectUri = $"{config.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={LarsCode}&requestType={entryPoint}";
        var locationQueryParam = !string.IsNullOrEmpty(location) ? $"&location={Uri.EscapeDataString(location)}" : string.Empty;

        return $"{config.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }
}
