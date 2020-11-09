﻿using AutoFixture.NUnit3;
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
                .WhichValue.Should().Be(request.Id.ToString());
        }

        [Test, AutoData]
        public void Then_Adds_Location_To_Dictionary(GetCourseProvidersRequest request)
        {
            request.ToDictionary().Should().ContainKey(nameof(GetCourseProvidersRequest.Location))
                .WhichValue.Should().Be(request.Location);
        }

        [Test, AutoData]
        public void Then_Adds_DeliveryModes_To_Dictionary(GetCourseProvidersRequest request)
        {
            var dictionary = request.ToDictionary();

            for (int i = 0; i < request.DeliveryModes.Count; i++)
            {
                dictionary.Should().ContainKey($"{nameof(GetCourseProvidersRequest.DeliveryModes)}[{i}]")
                    .WhichValue.Should().Be(request.DeliveryModes[i].ToString());
            }
        }

        [Test, AutoData]
        public void Then_Adds_ProviderRatings_To_Dictionary(GetCourseProvidersRequest request)
        {
            var dictionary = request.ToDictionary();

            for (int i = 0; i < request.ProviderRatings.Count; i++)
            {
                dictionary.Should().ContainKey($"{nameof(GetCourseProvidersRequest.ProviderRatings)}[{i}]")
                    .WhichValue.Should().Be(request.ProviderRatings[i].ToString());
            }
        }
    }
}
