using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetProviderDetails;

public class WhenGettingCourseProviderDetails
{
    [Test, MoqAutoData]
    public async Task Then_If_The_Query_Is_Valid_The_Service_Is_Called_And_The_Response_Is_Mapped_Correctly(
        GetCourseProviderDetailsQuery query,
        CourseProviderDetailsModel CourseProviderDetailsResponse,
        [Frozen] Mock<ICourseService> courseServiceMock,
        [Greedy] GetCourseProviderQueryHandler sut
    )
    {
        courseServiceMock.Setup(x => 
            x.GetCourseProvider(
                query.Ukprn, 
                query.LarsCode, 
                query.Location,
                query.Distance,
                query.ShortlistUserId.Value
            )
        ).ReturnsAsync(CourseProviderDetailsResponse);

        GetCourseProviderQueryResult result = await sut.Handle(query, CancellationToken.None);

        courseServiceMock.Verify(x => 
            x.GetCourseProvider(
                query.Ukprn, 
                query.LarsCode, 
                query.Location, 
                query.Distance,
                query.ShortlistUserId.Value
            ), 
            Times.Once
        );

        Assert.Multiple(() =>
        {
            Assert.That(result.Ukprn, Is.EqualTo(CourseProviderDetailsResponse.Ukprn));
            Assert.That(result.ProviderName, Is.EqualTo(CourseProviderDetailsResponse.ProviderName));
            Assert.That(result.ProviderAddress, Is.EqualTo(CourseProviderDetailsResponse.ProviderAddress));
            Assert.That(result.Contact, Is.EqualTo(CourseProviderDetailsResponse.Contact));
            Assert.That(result.CourseName, Is.EqualTo(CourseProviderDetailsResponse.CourseName));
            Assert.That(result.Level, Is.EqualTo(CourseProviderDetailsResponse.Level));
            Assert.That(result.LarsCode, Is.EqualTo(CourseProviderDetailsResponse.LarsCode));
            Assert.That(result.IFateReferenceNumber, Is.EqualTo(CourseProviderDetailsResponse.IFateReferenceNumber));
            Assert.That(result.Qar, Is.EqualTo(CourseProviderDetailsResponse.Qar));
            Assert.That(result.Reviews, Is.EqualTo(CourseProviderDetailsResponse.Reviews));
            Assert.That(result.EndpointAssessments, Is.EqualTo(CourseProviderDetailsResponse.EndpointAssessments));
            Assert.That(result.TotalProvidersCount, Is.EqualTo(CourseProviderDetailsResponse.TotalProvidersCount));
            Assert.That(result.ShortlistId, Is.EqualTo(CourseProviderDetailsResponse.ShortlistId));
            Assert.That(result.Locations, Is.EqualTo(CourseProviderDetailsResponse.Locations));
            Assert.That(result.Courses, Is.EqualTo(CourseProviderDetailsResponse.Courses));
            Assert.That(result.AnnualEmployerFeedbackDetails, Is.EqualTo(CourseProviderDetailsResponse.AnnualEmployerFeedbackDetails));
            Assert.That(result.AnnualApprenticeFeedbackDetails, Is.EqualTo(CourseProviderDetailsResponse.AnnualApprenticeFeedbackDetails));
        });
    }

    [Test, MoqAutoData]
    public async Task Then_If_There_Is_No_Course_Provider_Then_Service_Returns_Null(
        GetCourseProviderDetailsQuery query,
        [Frozen] Mock<ICourseService> courseServiceMock,
        [Greedy] GetCourseProviderQueryHandler sut
)
    {
        courseServiceMock.Setup(x =>
            x.GetCourseProvider(
                query.Ukprn,
                query.LarsCode,
                query.Location,
                query.Distance,
                query.ShortlistUserId.Value
            )
        ).ReturnsAsync((CourseProviderDetailsModel)null);

        GetCourseProviderQueryResult result = await sut.Handle(query, CancellationToken.None);

        courseServiceMock.Verify(x =>
            x.GetCourseProvider(
                query.Ukprn,
                query.LarsCode,
                query.Location,
                query.Distance,
                query.ShortlistUserId.Value
            ),
            Times.Once
        );

        Assert.That(result, Is.Null);
    }
}
