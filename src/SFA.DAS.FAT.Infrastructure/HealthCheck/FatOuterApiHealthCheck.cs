using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Infrastructure.Extensions;

namespace SFA.DAS.FAT.Infrastructure.HealthCheck
{
    public class FatOuterApiHealthCheck : IHealthCheck
    {
        private const string HealthCheckResultDescription = "FAT Outer Api check";

        private readonly IApiClient _apiClient;
        private readonly ILogger<FatOuterApiHealthCheck> _logger;

        public FatOuterApiHealthCheck(IApiClient apiClient, ILogger<FatOuterApiHealthCheck> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pinging FAT Outer API");

            var timer = Stopwatch.StartNew();
            var response = await _apiClient.Ping();
            timer.Stop();
            
            if(response == 200)
            {
                var durationString = timer.Elapsed.ToHumanReadableString();

                _logger.LogInformation($"FAT Outer API ping successful and took {durationString}");

                return HealthCheckResult.Healthy(HealthCheckResultDescription,
                    new Dictionary<string, object> { { "Duration", durationString } });
            }

            _logger.LogWarning($"FAT Outer API ping failed : [Code: {response}]");
            return HealthCheckResult.Unhealthy(HealthCheckResultDescription);
            
        }
    }
}