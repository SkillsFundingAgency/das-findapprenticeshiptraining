using System;
using MediatR;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;

public class GetCourseProviderDetailsQuery : IRequest<GetCourseProviderQueryResult>
{
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string Location { get; set; }
    public int? Distance { get; set; }
    public Guid? ShortlistUserId { get; set; }
}
