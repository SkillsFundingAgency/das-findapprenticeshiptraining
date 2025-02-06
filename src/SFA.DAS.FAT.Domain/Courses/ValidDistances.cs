using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses;

public static class ValidDistances
{
    private static readonly HashSet<int> _Distances = new() { 2, 5, 10, 15, 20, 30, 40, 50, 100 };
    public static IReadOnlyCollection<int> Distances => _Distances;

    public static bool IsValidDistance(int distance)
    {
        return _Distances.Contains(distance);
    }
}
