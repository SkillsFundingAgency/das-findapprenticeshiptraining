using System;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderQarModel
{
    public bool AchievementRatePresent { get; set; }

    public string PeriodStartYear { get; set; }

    public string PeriodEndYear { get; set; }

    public int Achievers { get; set; }

    public int TotalParticipantCount { get; set; }
    public string DidNotPassPercentage { get; set; }

    public string AchievementRate { get; set; }
    public string NationalAchievementRate { get; set; }

    public static implicit operator ProviderQarModel(GetProviderQarModel source)
    {
        if (source == null) return new ProviderQarModel();
        var periodStartYear = string.Empty;
        var periodEndYear = string.Empty;
        var achievers = 0;
        var totalParticipantCount = 0;
        var didNotPassPercentage = string.Empty;

        var isAchievementRateNumeric = double.TryParse(source.AchievementRate, out double achievementRateValue);
        var isLeaversCountNumeric = int.TryParse(source.Leavers, out int leaversValue);

        var achievementRatePresent = isAchievementRateNumeric && isLeaversCountNumeric && achievementRateValue != 0;

        if (source.Period is { Length: >= 4 })
        {
            periodStartYear = $"20{source.Period.AsSpan(0, 2)}";
            periodEndYear = $"20{source.Period.AsSpan(2, 2)}";
        }

        if (isAchievementRateNumeric && isLeaversCountNumeric && achievementRateValue != 0)
        {
            achievers = (int)Math.Round(leaversValue * (achievementRateValue / 100));
        }

        if (isLeaversCountNumeric)
        {
            totalParticipantCount = leaversValue;
        }

        if (isAchievementRateNumeric)
        {
            didNotPassPercentage = (100 - achievementRateValue).ToString("0.0");
        }

        return new ProviderQarModel
        {
            AchievementRatePresent = achievementRatePresent,
            PeriodStartYear = periodStartYear,
            PeriodEndYear = periodEndYear,
            Achievers = achievers,
            TotalParticipantCount = totalParticipantCount,
            DidNotPassPercentage = didNotPassPercentage,
            AchievementRate = source.AchievementRate,
            NationalAchievementRate = source.NationalAchievementRate
        };
    }
}
