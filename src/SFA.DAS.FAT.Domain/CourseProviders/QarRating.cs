using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.CourseProviders;
public enum QarRating
{
    [Description("Above 70%")]
    Excellent,
    [Description("60% to 70%")]
    Good,
    [Description("50% to 59%")]
    Poor,
    [Description("Less than 50%")]
    VeryPoor,
    [Description("No achievement rate")]
    None
}
