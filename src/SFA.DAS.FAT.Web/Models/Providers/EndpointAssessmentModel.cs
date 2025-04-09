using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class EndpointAssessmentModel
{

    public string CountFormatted { get; set; } = string.Empty;
    public string DetailsMessage { get; set; } = string.Empty;

    public static implicit operator EndpointAssessmentModel(GetProviderEndpointAssessmentsDetails source)
    {
        if (source == null) return new EndpointAssessmentModel();

        var endpointAssessmentCount = source.EndpointAssessmentCount;

        var countFormatted = endpointAssessmentCount.ToString("N0");

        string detailsMessage;

        if (endpointAssessmentCount <= 0 || source.EarliestAssessment == null)
        {
            detailsMessage =
                "apprentices have completed a course and taken their end-point assessment with this provider.";
        }
        else
        {
            var earliestYear = source.EarliestAssessment.Value.Year.ToString();

            detailsMessage = endpointAssessmentCount == 1
                ? $"apprentice has completed a course and taken their end-point assessment with this provider since {earliestYear}."
                : $"apprentices have completed a course and taken their end-point assessment with this provider since {earliestYear}.";
        }

        return new EndpointAssessmentModel
        {
            CountFormatted = countFormatted,
            DetailsMessage = detailsMessage
        };
    }
}
