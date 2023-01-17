using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.GetCourseProvidersRequestTests
{
    public class WhenCallingGetDictionary
    {
        [Test, AutoData]
        public void Then_Adds_Id_To_Dictionary(GetCourseProvidersRequest request)
        {
            request.ToDictionary().Should().ContainKey(nameof(GetCourseProvidersRequest.Id))
                .WhoseValue.Should().Be(request.Id.ToString());
        }

        [Test, AutoData]
        public void Then_Adds_Location_To_Dictionary(GetCourseProvidersRequest request)
        {
            request.ToDictionary().Should().ContainKey(nameof(GetCourseProvidersRequest.Location))
                .WhoseValue.Should().Be(request.Location);
        }

        [Test, AutoData]
        public void Then_Adds_DeliveryModes_To_Dictionary(GetCourseProvidersRequest request)
        {
            var dictionary = request.ToDictionary();

            for (int i = 0; i < request.DeliveryModes.Count; i++)
            {
                dictionary.Should().ContainKey($"{nameof(GetCourseProvidersRequest.DeliveryModes)}[{i}]")
                    .WhoseValue.Should().Be(request.DeliveryModes[i].ToString());
            }
        }

        [Test, AutoData]
        public void Then_Adds_EmployerProviderRatings_To_Dictionary(GetCourseProvidersRequest request)
        {
            var dictionary = request.ToDictionary();

            for (int i = 0; i < request.EmployerProviderRatings.Count; i++)
            {
                dictionary.Should().ContainKey($"{nameof(GetCourseProvidersRequest.EmployerProviderRatings)}[{i}]")
                    .WhoseValue.Should().Be(request.EmployerProviderRatings[i].ToString());
            }
        }


        [Test, AutoData]
        public void Then_Adds_ApprenticeProviderRatings_To_Dictionary(GetCourseProvidersRequest request)
        {
            var dictionary = request.ToDictionary();

            for (int i = 0; i < request.ApprenticeProviderRatings.Count; i++)
            {
                dictionary.Should().ContainKey($"{nameof(GetCourseProvidersRequest.ApprenticeProviderRatings)}[{i}]")
                    .WhoseValue.Should().Be(request.ApprenticeProviderRatings[i].ToString());
            }
        }
    }
}
