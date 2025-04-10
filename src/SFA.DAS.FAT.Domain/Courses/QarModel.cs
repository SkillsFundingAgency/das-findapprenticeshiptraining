using System;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class QarModel
{
    public string Period { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string NationalLeavers { get; set; }
    public string NationalAchievementRate { get; set; }
    public int TotalParticipants => TotalParticipantCount();
    public decimal FailureRate => GetFailureRate();
    public string QarPeriodStartYear { get => $"20{Period.AsSpan(0, 2)}"; }
    public string QarPeriodEndYear { get => $"20{Period.AsSpan(2, 2)}"; }
    public string PeriodDisplay => $"{QarPeriodStartYear} to {QarPeriodEndYear}";

    public int ConvertedLeavers
    {
        get
        {
            if (int.TryParse(Leavers, out int value))
            {
                return value;
            }

            return 0;
        }
    }

    public decimal? ConvertedAchievementRate
    {
        get
        {
            if (decimal.TryParse(AchievementRate, out decimal value))
            {
                return value;
            }

            return null;
        }
    }

    private int TotalParticipantCount()
    {
        if (ConvertedLeavers == 0 || ConvertedAchievementRate is null)
        {
            return 0;
        }

        decimal rate = ConvertedAchievementRate.Value;

        if (rate <= 0)
        {
            return ConvertedLeavers;
        }

        if (rate >= 100)
        {
            return this.ConvertedLeavers;
        }

        return (int)Math.Round(ConvertedLeavers / (rate / 100m));
    }

    private decimal GetFailureRate()
    {
        if (AchievementRate is null)
        {
            return 0;
        }

        return 100M - ConvertedAchievementRate.Value;
    }
}
