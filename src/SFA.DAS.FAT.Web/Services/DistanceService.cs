using System;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.Services;

public static class DistanceService
{
    public const string AcrossEnglandFilterValue = "All";
    public const int DefaultDistance = 1000;
    public const int TenMiles = 10;
    public const string AcrossEnglandDisplayText = "across England";

    private static readonly HashSet<int> _Distances = new() { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
    public static IReadOnlyCollection<int> Distances => _Distances;

    public static int GetValidDistance(string distance)
    {
        if (string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase))
        {
            return DefaultDistance;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return validDistance;
        }

        return DefaultDistance;
    }

    public static int GetValidDistance(string distance, string location)
    {
        if (string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase))
        {
            return DefaultDistance;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return string.IsNullOrWhiteSpace(location) ? DefaultDistance : validDistance;
        }

        return TenMiles;
    }

    public static int? GetValidDistanceNullable(string distance)
    {
        if (string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase))
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
        if (string.IsNullOrWhiteSpace(location) || string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase))
        {
            return AcrossEnglandFilterValue;
        }

        if (int.TryParse(distance, out int validDistance) && _Distances.Contains(validDistance))
        {
            return validDistance.ToString();
        }

        return AcrossEnglandFilterValue;
    }

    public static bool IsValidDistance(string distance)
    {
        if (string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase))
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
