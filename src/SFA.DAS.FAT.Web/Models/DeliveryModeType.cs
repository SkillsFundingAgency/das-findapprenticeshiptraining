using System.ComponentModel;

namespace SFA.DAS.FAT.Web.Models;

public enum DeliveryModeType
{
    [Description("At apprentice’s workplace")]
    Workplace = 0,
    [Description("Day release")]
    DayRelease = 1,
    [Description("Block release")]
    BlockRelease = 2,
    [Description("Not Found")]
    NotFound = 3,
    [Description("National coverage")]
    National = 4,
}
