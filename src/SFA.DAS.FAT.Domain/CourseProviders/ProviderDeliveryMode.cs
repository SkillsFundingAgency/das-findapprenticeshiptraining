using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.CourseProviders;
public enum ProviderDeliveryMode
{
    [Description("At apprentice's workplace")]
    Workplace = 0,
    [Description("At provider's location")]
    Provider = 1,
    [Description("Day release")]
    DayRelease = 2,
    [Description("Block release")]
    BlockRelease = 3

}
