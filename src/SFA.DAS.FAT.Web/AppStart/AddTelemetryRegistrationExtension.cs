using System.Diagnostics.CodeAnalysis;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.FAT.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddTelemetryRegistrationExtension
{
    private static readonly string AppInsightsConnectionString = "APPLICATIONINSIGHTS_CONNECTION_STRING";

    public static IServiceCollection AddTelemetryRegistration(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddOpenTelemetry().UseAzureMonitor(options =>
        {
            options.ConnectionString = AppInsightsConnectionString;
        });

        return services;
    }
}
