using System;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class QarModel
{
    public string Period { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string NationalLeavers { get; set; }
    public string NationalAchievementRate { get; set; }
    public int TotalNumberOfCompletedParticipants => GetTotalNumberOfCompletedParticipants();
    public decimal FailureRate => GetFailureRate();
    public string QarPeriodStartYear { get => $"20{Period.AsSpan(0, 2)}"; }
    public string QarPeriodEndYear { get => $"20{Period.AsSpan(2, 2)}"; }
    public string PeriodDisplay => $"{QarPeriodStartYear} to {QarPeriodEndYear}";

    public int ConvertedLeavers
    {
        get
        {
            return int.TryParse(Leavers, out int value) ? value : 0;
        }
    }

    public decimal? ConvertedAchievementRate
    {
        get
        {
            return decimal.TryParse(AchievementRate, out decimal value) ? value : null;
        }
    }

    private int GetTotalNumberOfCompletedParticipants()
    {
        int totalParticipants = ConvertedLeavers;

        decimal? percentageThatAchieved = ConvertedAchievementRate;

        if (totalParticipants == 0 || percentageThatAchieved is null)
        {
            return 0;
        }

        decimal rate = percentageThatAchieved.Value;

        if (rate <= 0)
        {
            return 0;
        }

        if (rate >= 100)
        {
            return totalParticipants;
        }

        return (int)Math.Round(totalParticipants * (rate / 100m));
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
