using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Extensions;

public static class LearningTypeExtensions
{
    public static string GetTagClass(this LearningType learningType)
    {
        return learningType switch
        {
            LearningType.Apprenticeship => "govuk-tag--blue",
            LearningType.FoundationApprenticeship => "govuk-tag--pink",
            LearningType.ApprenticeshipUnit => "govuk-tag--purple",
            _ => string.Empty
        };
    }
}
