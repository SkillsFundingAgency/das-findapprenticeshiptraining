﻿using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Services
{
    public class WhenGettingShortlistForUser
    {
        [Test, MoqAutoData]
        public async Task Then_The_Data_Is_Returned_From_The_Api(
            Guid shortlistUserId,
            Domain.Shortlist.ShortlistForUser apiResponse,
            [Frozen] Mock<IApiClient> apiClient,
            ShortlistService service)
        {
            //Arrange
            apiClient.Setup(x =>
                    x.Get<Domain.Shortlist.ShortlistForUser>(
                        It.Is<GetShortlistForUserApiRequest>(c => c.GetUrl.Contains(shortlistUserId.ToString(), StringComparison.InvariantCultureIgnoreCase))))
                .ReturnsAsync(apiResponse);
            
            //Act
            var actual = await service.GetShortlistForUser(shortlistUserId);
            
            //Assert
            actual.Should().BeEquivalentTo(apiResponse);
        }
    }
}
