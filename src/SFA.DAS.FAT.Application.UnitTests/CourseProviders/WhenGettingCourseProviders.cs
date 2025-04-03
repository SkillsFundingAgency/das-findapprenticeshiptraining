using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Responses;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.CourseProviders;

public class WhenGettingCourseProviders
{
    [Test, MoqAutoData]
    public async Task Then_The_CourseService_And_AcademicLatestYearsService_Returns_Expected_Details(
        GetCourseProvidersQuery query,
        GetAcademicYearsLatestResponse academicYearsLatestResponse,
        CourseProvidersDetails courseProvidersDetails,
        [Frozen] Mock<ICourseService> mockCourseService,
        [Frozen] Mock<IAcademicYearsService> mockAcademicYearsService,
        GetCourseProvidersQueryHandler sut,
        CancellationToken cancellationToken
        )

    {
        query.OrderBy = ProviderOrderBy.Distance;

        mockCourseService.Setup(s => s.GetCourseProviders(It.IsAny<CourseProvidersParameters>())
        ).ReturnsAsync(courseProvidersDetails);

        mockAcademicYearsService.Setup(a => a.GetAcademicYearsLatestAsync(cancellationToken))
            .ReturnsAsync(academicYearsLatestResponse);

        var expectedResult = new GetCourseProvidersResult
        {
            LarsCode = courseProvidersDetails.LarsCode,
            Page = courseProvidersDetails.Page,
            PageSize = courseProvidersDetails.PageSize,
            TotalPages = courseProvidersDetails.TotalPages,
            TotalCount = courseProvidersDetails.TotalCount,
            StandardName = courseProvidersDetails.StandardName,
            QarPeriod = academicYearsLatestResponse.QarPeriod,
            ReviewPeriod = academicYearsLatestResponse.ReviewPeriod,
            Providers = courseProvidersDetails.Providers
        };

        var result = await sut.Handle(query, cancellationToken);

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    [InlineAutoData(ProviderOrderBy.Distance)]
    [InlineAutoData(ProviderOrderBy.AchievementRate)]
    [InlineAutoData(ProviderOrderBy.ApprenticeProviderRating)]
    [InlineAutoData(ProviderOrderBy.EmployerProviderRating)]
    public async Task Then_The_OrderBy_Values_Are_Used_In_Calling_CourseService(
        ProviderOrderBy orderBy,
        GetCourseProvidersQuery query,
        GetAcademicYearsLatestResponse academicYearsLatestResponse,
        CourseProvidersDetails courseProvidersDetails,
        CancellationToken cancellationToken
    )
    {
        Mock<ICourseService> mockCourseService = new Mock<ICourseService>();
        Mock<IAcademicYearsService> mockAcademicYearsService = new Mock<IAcademicYearsService>();

        query.OrderBy = orderBy;
        CourseProvidersParameters courseProvidersParameters = GetCourseProvidersParameters(query);
        mockCourseService.Setup(s => s.GetCourseProviders(It.IsAny<CourseProvidersParameters>())
        ).ReturnsAsync(courseProvidersDetails);

        mockAcademicYearsService.Setup(a => a.GetAcademicYearsLatestAsync(cancellationToken))
            .ReturnsAsync(academicYearsLatestResponse);

        GetCourseProvidersQueryHandler sut =
            new GetCourseProvidersQueryHandler(mockCourseService.Object, mockAcademicYearsService.Object);

        await sut.Handle(query, cancellationToken);

        mockCourseService.Verify(x => x.GetCourseProviders(It.Is<CourseProvidersParameters>(
            c => c.Id == query.Id
            && c.OrderBy == orderBy
            && c.Distance == query.Distance
            && c.Location == query.Location
            && c.DeliveryModeTypes == query.DeliveryModes
            && c.EmployerProviderRatingTypes == query.EmployerProviderRatings
            && c.ApprenticeProviderRatingTypes == query.ApprenticeProviderRatings
            && c.QarRatings == query.Qar
            && c.Page == query.Page
            && c.ShortlistUserId == query.ShortlistUserId
            )), Times.Once);
    }

    private static CourseProvidersParameters GetCourseProvidersParameters(GetCourseProvidersQuery request)
    {
        return new CourseProvidersParameters
        {
            Id = request.Id,
            OrderBy = request.OrderBy ?? ProviderOrderBy.Distance,
            Distance = request.Distance,
            Location = request.Location,
            DeliveryModeTypes = request.DeliveryModes,
            EmployerProviderRatingTypes = request.EmployerProviderRatings,
            ApprenticeProviderRatingTypes = request.ApprenticeProviderRatings,
            QarRatings = request.Qar,
            Page = request.Page,
            ShortlistUserId = request.ShortlistUserId
        };
    }
}
