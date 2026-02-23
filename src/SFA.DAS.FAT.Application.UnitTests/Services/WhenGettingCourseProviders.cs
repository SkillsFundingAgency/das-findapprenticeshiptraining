#nullable enable
using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.CourseProviders.Api;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public class WhenGettingCourseProviders
{
    private const string BaseUrl = "BaseUrl";

    [Test, MoqAutoData]
    public async Task GetCourseProviders_WithValidParameters_CallsApiClientWithCorrectUrl(
        GetCourseProvidersQuery query,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        CourseService service)
    {
        var courseProvidersParams = new CourseProvidersParameters
        {
            LarsCode = query.LarsCode,
            OrderBy = ProviderOrderBy.Distance,
            Distance = query.Distance,
            Location = query.Location,
            DeliveryModeTypes = query.DeliveryModes?.ToList(),
            EmployerProviderRatingTypes = query.EmployerProviderRatings.ToList(),
            ApprenticeProviderRatingTypes = query.ApprenticeProviderRatings.ToList(),
            QarRatings = query.Qar.ToList(),
            Page = query.Page,
            ShortlistUserId = query.ShortlistUserId
        };

        var expectedUrl = new CourseProvidersApiRequest(mockConfig.Object.Value.BaseUrl, courseProvidersParams).GetUrl;

        await service.GetCourseProviders(courseProvidersParams);

        mockApiClient.Verify(client => client.Get<CourseProvidersDetails>(
            It.Is<CourseProvidersApiRequest>(request => request.GetUrl == expectedUrl)));
    }

    [Test, MoqAutoData]
    public async Task GetCourseProviders_WhenApiReturnsData_ReturnsProviderDetails(
        GetCourseProvidersQuery query,
        CourseProvidersDetails providersFromApi,
        [Frozen] Mock<IApiClient> mockApiClient,
        CourseService service)
    {
        mockApiClient
            .Setup(client => client.Get<CourseProvidersDetails>(
                It.IsAny<CourseProvidersApiRequest>()))
            .ReturnsAsync(providersFromApi);

        var courseProvidersParams = new CourseProvidersParameters
        {
            LarsCode = query.LarsCode,
            OrderBy = ProviderOrderBy.Distance,
            Distance = query.Distance,
            Location = query.Location,
            DeliveryModeTypes = query.DeliveryModes?.ToList(),
            EmployerProviderRatingTypes = query.EmployerProviderRatings.ToList(),
            ApprenticeProviderRatingTypes = query.ApprenticeProviderRatings.ToList(),
            QarRatings = query.Qar.ToList(),
            Page = query.Page,
            ShortlistUserId = query.ShortlistUserId
        };

        var response = await service.GetCourseProviders(courseProvidersParams);


        response.Should().BeEquivalentTo(providersFromApi);
    }

    [Test, MoqAutoData]
    public async Task GetCourseProviders_WhenApiReturns404_ReturnsNull(
        [Frozen] Mock<IApiClient> mockApiClient,
        CourseService service)
    {
        HttpRequestException exception = new HttpRequestException("message", null, HttpStatusCode.NotFound);

        mockApiClient.Setup(x => x.Get<CourseProvidersDetails>(
                It.IsAny<CourseProvidersApiRequest>()))
            .ThrowsAsync(exception);

        var response = await service.GetCourseProviders(new CourseProvidersParameters());

        response.Should().BeNull();
    }

    [Test, AutoData]
    public void GetUrl_WithAllParameters_ConstructsUrlCorrectly(string baseUrl, string id, ProviderOrderBy orderBy, int distance, string location, List<ProviderDeliveryMode> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes,
        List<QarRating> qarRatings,
        int page, Guid shortlistUserId)
    {
        //Arrange
        var courseProvidersParams = new CourseProvidersParameters
        {
            LarsCode = id,
            OrderBy = orderBy,
            Distance = distance,
            Location = location,
            DeliveryModeTypes = deliveryModeTypes,
            EmployerProviderRatingTypes = employerProviderRatingTypes,
            ApprenticeProviderRatingTypes = apprenticeProviderRatingTypes,
            QarRatings = qarRatings,
            Page = page,
            ShortlistUserId = shortlistUserId
        };

        var actual = new CourseProvidersApiRequest(baseUrl, courseProvidersParams);

        var pageParam = string.Empty;
        if (page > 1) pageParam = $"&page={page}";

        //Assert
        actual.GetUrl.Should().Be($"{baseUrl}courses/{id}/providers?orderBy={orderBy}&distance={distance}&location={location}&" +
              $"deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&employerProviderRatings=" +
              $"{string.Join("&employerProviderRatings=", employerProviderRatingTypes)}&" +
              $"apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}&" +
              $"qar={string.Join("&qar=", qarRatings)}{pageParam}&pageSize={Constants.DefaultPageSize}&shortlistUserId={shortlistUserId}");
    }

    [TestCase(null, null, null, null, null, null, 1, null, "&pageSize=10")]
    [TestCase(5, null, null, null, null, null, 1, null, "&distance=5&pageSize=10")]
    [TestCase(null, "loc", null, null, null, null, null, null, "&location=loc&pageSize=10")]
    [TestCase(null, null, ProviderDeliveryMode.Provider, null, null, null, null, null, "&deliveryModes=Provider&pageSize=10")]
    [TestCase(null, null, null, ProviderRating.VeryPoor, null, null, null, null, "&employerProviderRatings=VeryPoor&pageSize=10")]
    [TestCase(null, null, null, null, ProviderRating.VeryPoor, null, null, null, "&apprenticeProviderRatings=VeryPoor&pageSize=10")]
    [TestCase(null, null, null, null, null, QarRating.Excellent, null, null, "&qar=Excellent&pageSize=10")]
    [TestCase(null, null, null, null, null, null, 2, null, "&page=2&pageSize=10")]
    [TestCase(null, null, null, null, null, null, 10, null, "&page=10&pageSize=10")]
    [TestCase(null, null, null, null, null, null, null, "3f616821-64a2-4dda-97cd-138f428d26b5", "&pageSize=10&shortlistUserId=3f616821-64a2-4dda-97cd-138f428d26b5")]
    [TestCase(5, "loc", ProviderDeliveryMode.DayRelease, ProviderRating.VeryPoor, ProviderRating.Good, QarRating.Good, 25, "3f616821-64a2-4dda-97cd-138f428d26b5",
             "&distance=5&location=loc" +
             "&deliveryModes=DayRelease" +
             "&employerProviderRatings=VeryPoor" +
             "&apprenticeProviderRatings=Good" +
             "&qar=Good" +
             "&page=25&pageSize=10" +
             "&shortlistUserId=3f616821-64a2-4dda-97cd-138f428d26b5")]
    public void GetUrl_WithVariousParameterCombinations_ConstructsUrlCorrectly(int? distance, string? location, ProviderDeliveryMode? deliveryModeType, ProviderRating? employerProviderRating, ProviderRating? apprenticeProviderRating,
         QarRating? qarRating,
         int? page, Guid? shortlistUserId, string expectedUrl)
    {
        //Arrange Act
        page ??= 1;

        var id = "1";
        var orderBy = ProviderOrderBy.Distance;

        var deliveryModeTypes = new List<ProviderDeliveryMode>();
        if (deliveryModeType != null) deliveryModeTypes.Add((ProviderDeliveryMode)deliveryModeType);

        var employerProviderRatings = new List<ProviderRating>();
        if (employerProviderRating != null) employerProviderRatings.Add((ProviderRating)employerProviderRating);

        var apprenticeProviderRatings = new List<ProviderRating>();
        if (apprenticeProviderRating != null) apprenticeProviderRatings.Add((ProviderRating)apprenticeProviderRating);

        var qarRatings = new List<QarRating>();
        if (qarRating != null) qarRatings.Add((QarRating)qarRating);


        var expectedFullUrl = $"{BaseUrl}courses/{id}/providers?orderBy={orderBy}{expectedUrl}";


        var courseProvidersParams = new CourseProvidersParameters
        {
            LarsCode = id,
            OrderBy = orderBy,
            Distance = distance,
            Location = location,
            DeliveryModeTypes = deliveryModeTypes,
            EmployerProviderRatingTypes = employerProviderRatings,
            ApprenticeProviderRatingTypes = apprenticeProviderRatings,
            QarRatings = qarRatings,
            Page = (int)page,
            ShortlistUserId = shortlistUserId
        };
        var actual = new CourseProvidersApiRequest(BaseUrl, courseProvidersParams);

        //Assert
        actual.GetUrl.Should().Be($"{expectedFullUrl}");
    }
}

