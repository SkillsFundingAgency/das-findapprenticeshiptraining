using System;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class EmployerProviderAttribute
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Weakness { get; set; }

    public int TotalCount => Strength + Weakness;
    public int StrengthPerc => (int)((TotalCount == 0) ? 0 : Math.Round(((double)Strength) / TotalCount * 100, 0));

    public int WeaknessPerc => (int)((TotalCount == 0) ? 0 : Math.Round(((double)Weakness) / TotalCount * 100, 0));
}


public class ApprenticeProviderAttribute
{
    public string Name { get; set; }

    public string Category { get; set; }
    public int Agree { get; set; }
    public int Disagree { get; set; }

    public int TotalCount => Agree + Disagree;
    public int AgreePerc => (int)((TotalCount == 0) ? 0 : Math.Round(((double)Agree) / TotalCount * 100, 0));

    public int DisagreePerc => (int)((TotalCount == 0) ? 0 : Math.Round(((double)Disagree) / TotalCount * 100, 0));
}
