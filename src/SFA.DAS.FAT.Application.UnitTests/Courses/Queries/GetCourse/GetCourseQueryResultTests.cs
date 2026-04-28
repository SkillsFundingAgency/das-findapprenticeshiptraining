using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourse;

public class GetCourseQueryResultTests
{
    [Test, AutoData]
    public void Operator_ConvertsFromApiResponse(GetCourseResponse apiResponse)
    {
        GetCourseQueryResult sut = apiResponse;

        sut.Should().BeEquivalentTo(apiResponse, options => options.ExcludingMissingMembers());
    }
}
