using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourseProviders
{
    public class WhenGettingCourseProviders
    {
        [Test, MoqAutoData]
        public async Task Then_Returns_Results_From_Service(
            GetCourseProvidersQuery query,
            TrainingCourseProviders providersFromService,
            [Frozen] Mock<ICourseService> mockService,
            GetCourseProvidersQueryHandler handler)
        {
            mockService
                .Setup(service => service.GetCourseProviders(query.CourseId, query.Location, query.DeliveryModes, query.EmployerProviderRatings, query.ApprenticeProviderRatings, query.Lat, query.Lon, query.ShortlistUserId))
                .ReturnsAsync(providersFromService);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Course.Should().BeEquivalentTo(providersFromService.Course);
            result.Providers.Should().BeEquivalentTo(providersFromService.CourseProviders);
            result.Total.Should().Be(providersFromService.Total);
            result.TotalFiltered.Should().Be(providersFromService.TotalFiltered);
            result.Location.Should().Be(providersFromService.Location.Name);
            result.LocationGeoPoint.Should().BeEquivalentTo(providersFromService.Location.LocationPoint.GeoPoint);
            result.ShortlistItemCount.Should().Be(providersFromService.ShortlistItemCount);
            result.ShowEmployerDemand.Should().Be(providersFromService.ShowEmployerDemand);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Results_From_Service_With_No_Location(
            GetCourseProvidersQuery query,
            TrainingCourseProviders providersFromService,
            [Frozen] Mock<ICourseService> mockService,
            GetCourseProvidersQueryHandler handler)
        {
            providersFromService.Location = null;
            mockService
                .Setup(service => service.GetCourseProviders(query.CourseId, query.Location, query.DeliveryModes, query.EmployerProviderRatings, query.ApprenticeProviderRatings, query.Lat, query.Lon, query.ShortlistUserId))
                .ReturnsAsync(providersFromService);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Course.Should().BeEquivalentTo(providersFromService.Course);
            result.Providers.Should().BeEquivalentTo(providersFromService.CourseProviders);
            result.Total.Should().Be(providersFromService.Total);
            result.Location.Should().BeNullOrEmpty();
            result.LocationGeoPoint.Should().BeNull();
        }

    }
}
