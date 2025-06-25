using System;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Domain.Courses;

public class Ksb
{
    public string Type { get; set; }
    public Guid Id { get; set; }
    public string Detail { get; set; }

    public static implicit operator Ksb(KsbResponse standard)
    {
        return new Ksb
        {
            Type = standard.Type,
            Id = standard.Id,
            Detail = standard.Detail
        };
    }
}
