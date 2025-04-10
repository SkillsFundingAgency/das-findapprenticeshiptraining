using System;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class AnnualEmployerFeedbackDetailsModel
{
    public long Ukprn { get; set; }

    public int Stars { get; set; }

    public int ReviewCount { get; set; }

    public string TimePeriod { get; set; }

    public IEnumerable<ProviderAttributeModel> ProviderAttribute { get; set; }

    public string TimePeriodDisplayText => GetConvertedTimePeriod();

    private string GetConvertedTimePeriod()
    {
        if (string.IsNullOrWhiteSpace(TimePeriod))
        {
            return string.Empty;
        }

        if (TimePeriod.Equals("All", StringComparison.OrdinalIgnoreCase) || TimePeriod.Equals("Overall", StringComparison.OrdinalIgnoreCase))
        {
            return "Overall reviews";
        }

        ReadOnlySpan<char> timePeriodSpan = TimePeriod.AsSpan(2);

        if (timePeriodSpan.Length == 4 &&
            int.TryParse(timePeriodSpan.Slice(0, 2), out int startYearPart) &&
            int.TryParse(timePeriodSpan.Slice(2, 2), out int endYearPart))
        {
            int startYear = 2000 + startYearPart;
            int endYear = 2000 + endYearPart;

            if (endYear == DateTime.UtcNow.Year)
            {
                return $"{startYear} to today";
            }

            return $"{startYear} to {endYear}";
        }

        return TimePeriod;
    }
}
