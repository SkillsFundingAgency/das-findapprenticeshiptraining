using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.Courses;


public enum ProviderRating
{
    [Description("Excellent")]
    Excellent,
    [Description("Good")]
    Good,
    [Description("Poor")]
    Poor,
    [Description("Very poor")]
    VeryPoor,
    [Description("Not yet reviewed")]
    NotYetReviewed
}
