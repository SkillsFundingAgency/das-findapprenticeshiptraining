using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.Courses;
public enum ApprenticeshipType
{
    [Description("Standard")]
    Apprenticeship,
    [Description("Foundation")]
    FoundationApprenticeship
}
