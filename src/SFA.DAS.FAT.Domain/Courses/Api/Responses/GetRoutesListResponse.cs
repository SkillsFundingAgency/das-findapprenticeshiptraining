using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses.Api.Responses;

public sealed class GetRoutesListResponse
{
    public IEnumerable<Route> Routes { get; set; }
}
