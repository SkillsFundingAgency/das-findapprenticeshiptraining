using System;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.Services;

public static class DistanceService
{
    private static readonly HashSet<int> _Distances = new() { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
    public static IReadOnlyCollection<int> Distances => _Distances;

    public const string ACROSS_ENGLAND_FILTER_VALUE = "All";

    public const int DEFAULT_DISTANCE = 1000;

    public const int TEN_MILES = 10;

    public const string ACROSS_ENGLAND_DISPLAY_TEXT = "across England";

    public static int GetValidDistance(string distance)
    {
        if (string.Equals(distance, ACROSS_ENGLAND_FILTER_VALUE, StringComparison.OrdinalIgnoreCase))
        {
            return DEFAULT_DISTANCE;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return validDistance;
        }

        return DEFAULT_DISTANCE;
    }

    public static int GetValidDistance(string distance, string location)
    {
        if (string.Equals(distance, ACROSS_ENGLAND_FILTER_VALUE, StringComparison.OrdinalIgnoreCase))
        {
            return DEFAULT_DISTANCE;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return string.IsNullOrWhiteSpace(location) ? DEFAULT_DISTANCE : validDistance;
        }

        return TEN_MILES;
    }

    public static int? GetValidDistanceNullable(string distance)
    {
        if (string.Equals(distance, ACROSS_ENGLAND_FILTER_VALUE, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return validDistance;
        }

        return null;
    }

    public static string GetDistanceQueryString(string distance, string location)
    {
        if (string.IsNullOrWhiteSpace(location) || string.Equals(distance, ACROSS_ENGLAND_FILTER_VALUE, StringComparison.OrdinalIgnoreCase))
        {
            return ACROSS_ENGLAND_FILTER_VALUE;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return validDistance.ToString();
        }

        return ACROSS_ENGLAND_FILTER_VALUE;
    }

    public static bool IsValidDistance(string distance)
    {
        if (string.Equals(distance, ACROSS_ENGLAND_FILTER_VALUE, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return true;
        }

        return false;
    }
}
