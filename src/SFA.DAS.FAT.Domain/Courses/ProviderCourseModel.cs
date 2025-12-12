namespace SFA.DAS.FAT.Domain.Courses;

public sealed class ProviderCourseModel
{
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string LarsCode { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string NameAndLevel => $"{CourseName} (level {Level})";
}
