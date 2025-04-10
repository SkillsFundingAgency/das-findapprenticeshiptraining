using System;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class EndpointAssessmentModel
{
    public DateTime? EarliestAssessment { get; }
    public int EndpointAssessmentCount { get; }

    public EndpointAssessmentModel(DateTime? earliestAssessment, int endpointAssessmentCount)
    {
        EarliestAssessment = earliestAssessment;
        EndpointAssessmentCount = endpointAssessmentCount;
    }
}
