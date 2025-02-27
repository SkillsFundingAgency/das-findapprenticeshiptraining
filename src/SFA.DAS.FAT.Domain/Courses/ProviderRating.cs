using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.Courses;


public enum ApprenticeProviderRating
{
    [Description("Excellent")]
    Excellent,
    [Description("Good")]
    Good,
    [Description("Poor")]
    Poor,
    [Description("Very poor")]
    VeryPoor,
    [Description("No apprentice reviews")]
    NotYetReviewed
}

public enum EmployerProviderRating
{
    [Description("Excellent")]
    Excellent,
    [Description("Good")]
    Good,
    [Description("Poor")]
    Poor,
    [Description("Very poor")]
    VeryPoor,
    [Description("No employer reviews")]
    NotYetReviewed
}

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
    NotYetReviewed
}
