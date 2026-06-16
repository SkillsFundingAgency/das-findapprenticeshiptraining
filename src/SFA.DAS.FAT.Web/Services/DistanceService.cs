using System;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.Services;

public static class DistanceService
{
    public const string AcrossEnglandFilterValue = "All";
    public const int DefaultDistance = 1000;
    public const int TenMiles = 10;
    public const string AcrossEnglandDisplayText = "across England";

    private static readonly HashSet<int> _distances = new() { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
    public static IReadOnlyCollection<int> Distances => _distances;

    public static int GetValidDistance(string distance)
    {
        if (IsAcrossEngland(distance))
        {
            return DefaultDistance;
        }

        return TryGetValidDistance(distance, out var validDistance)
            ? validDistance
            : DefaultDistance;
    }

    public static int GetValidDistance(string distance, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return TenMiles;
        }

        if (IsAcrossEngland(distance))
        {
            return DefaultDistance;
        }

        return TryGetValidDistance(distance, out var validDistance)
            ? validDistance
            : TenMiles;
    }

    public static int? GetValidDistanceNullable(string distance)
    {
        if (IsAcrossEngland(distance))
        {
            return null;
        }

        return TryGetValidDistance(distance, out var validDistance)
            ? validDistance
            : null;
    }

    public static string GetDistance(string distance, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return TenMiles.ToString();
        }

        return IsAcrossEngland(distance) ? AcrossEnglandFilterValue
               : TryGetValidDistance(distance, out var validDistance) ? validDistance.ToString() : AcrossEnglandFilterValue;
    }

    public static bool IsValidDistance(string distance) =>
        IsAcrossEngland(distance) ||
        TryGetValidDistance(distance, out _);

    public static bool IsAcrossEngland(string distance) =>
        string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase);

    private static bool TryGetValidDistance(string distance, out int validDistance) =>
        int.TryParse(distance, out validDistance) && _distances.Contains(validDistance);
}
