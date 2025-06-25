using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

public record RelatedOccupation(string Title, int Level)
{
    public static implicit operator RelatedOccupation(RelatedOccupationResponse standard)
    {
        return new RelatedOccupation(standard.Title, standard.Level);
    }
}
