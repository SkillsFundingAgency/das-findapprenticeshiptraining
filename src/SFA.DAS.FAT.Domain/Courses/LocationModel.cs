using System.Text;
using SFA.DAS.FAT.Domain.CourseProviders;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class LocationModel
{
    public long Ordering { get; set; }
    public bool AtEmployer { get; set; }
    public bool BlockRelease { get; set; }
    public bool DayRelease { get; set; }
    public LocationType LocationType { get; set; }
    public string CourseLocation { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
    public double CourseDistance { get; set; }
}

public static class LocationModelExtensions
{
    public static string FormatAddress(this LocationModel location)
    {
        var builder = new StringBuilder();

        void AppendIfNotEmpty(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(value.Trim());
            }
        }

        AppendIfNotEmpty(location.AddressLine1);
        AppendIfNotEmpty(location.AddressLine2);
        AppendIfNotEmpty(location.Town);
        AppendIfNotEmpty(location.County);
        AppendIfNotEmpty(location.Postcode);

        return builder.ToString();
    }
}
