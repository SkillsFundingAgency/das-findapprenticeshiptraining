using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.CourseProviders;

public enum ProviderDeliveryMode
{
    [Description("At learner's workplace")]
    Workplace = 0,
    [Description("At training provider's location")]
    Provider = 1,
    [Description("Day release")]
    DayRelease = 2,
    [Description("Block release")]
    BlockRelease = 3,
    [Description("Online")]
    Online = 4
}
