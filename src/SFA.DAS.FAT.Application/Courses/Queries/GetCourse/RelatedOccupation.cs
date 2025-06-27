using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

public class RelatedOccupation
{
    public string Title { get; set; }
    public int Level { get; set; }
    public string Description => $"{Title} (level {Level})";

    public static implicit operator RelatedOccupation(RelatedOccupationResponse standard)
    {
        return new RelatedOccupation { Title = standard.Title, Level = standard.Level };
    }
}
