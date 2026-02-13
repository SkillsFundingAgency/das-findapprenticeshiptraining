using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.Courses;

public enum ApprenticeshipType
{
    [Description("Apprenticeships")]
    Apprenticeship,
    [Description("Foundation apprenticeships")]
    FoundationApprenticeship,
    [Description("Apprenticeship units")]
    ApprenticeshipUnit
}
