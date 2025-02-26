using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses.Api.Responses;

public sealed class GetLevelsListResponse
{
    public IEnumerable<Level> Levels { get; set; }
}
