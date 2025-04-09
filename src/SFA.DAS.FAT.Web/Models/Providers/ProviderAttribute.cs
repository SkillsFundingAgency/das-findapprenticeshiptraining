using System;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderAttribute
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Weakness { get; set; }

    public int TotalCount => Strength + Weakness;
    public int StrengthPerc => (int)((TotalCount == 0) ? 0 : Math.Round(((double)Strength) / TotalCount * 100, 0));

    public int WeaknessPerc => (int)((TotalCount == 0) ? 0 : Math.Round(((double)Weakness) / TotalCount * 100, 0));
}
