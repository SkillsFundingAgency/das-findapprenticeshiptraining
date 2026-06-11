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

    public static int GetValidDistance(string distance) =>
        ResolveDistance(distance, DefaultDistance);

    public static int GetValidDistance(string distance, string location)
    {
        if (!HasLocation(location))
        {
            return TenMiles;
        }

        return ResolveDistance(distance, TenMiles);
    }

    public static int? GetValidDistanceNullable(string distance)
    {
        if (IsAcrossEngland(distance))
        {
            return null;
        }

        return TryGetDistance(distance, out var validDistance)
            ? validDistance
            : null;
    }

    public static string GetDistance(string distance, string location)
    {
        if (!HasLocation(location))
        {
            return TenMiles.ToString();
        }

        return IsAcrossEngland(distance)
            ? AcrossEnglandFilterValue
            : TryGetDistance(distance, out var validDistance)
                ? validDistance.ToString()
                : AcrossEnglandFilterValue;
    }

    public static bool IsValidDistance(string distance) =>
        IsAcrossEngland(distance) || TryGetDistance(distance, out _);

    public static string EnsureHasDefaultDistance(string distance) =>
        string.IsNullOrWhiteSpace(distance) || !IsValidDistance(distance)
            ? TenMiles.ToString()
            : distance;

    public static int? GetConvertedDistanceForDetails(string distance, string location)
    {
        if (!IsValidDistance(distance))
        {
            return TenMiles;
        }

        if (IsAcrossEngland(distance))
        {
            return DefaultDistance;
        }

        return HasLocation(location) ? GetValidDistance(distance) : TenMiles;
    }

    private static int ResolveDistance(string distance, int fallbackDistance)
    {
        if (IsAcrossEngland(distance))
        {
            return DefaultDistance;
        }

        return TryGetDistance(distance, out var validDistance)
            ? validDistance
            : fallbackDistance;
    }

    private static bool HasLocation(string location) =>
        !string.IsNullOrWhiteSpace(location);

    private static bool IsAcrossEngland(string distance) =>
        string.Equals(distance, AcrossEnglandFilterValue, StringComparison.OrdinalIgnoreCase);

    private static bool TryGetDistance(string distance, out int validDistance) =>
        int.TryParse(distance, out validDistance) && _distances.Contains(validDistance);
}
