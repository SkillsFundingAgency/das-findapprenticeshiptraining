﻿using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Providers.Query.GetProvider;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models.Providers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ProvidersControllerTests;

public class WhenGettingProviders
{
    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved(
        int ukprn,
        GetProviderQueryResponse response,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IDateTimeService> dateTimeServiceMock,
        [Greedy] ProvidersController controller)
    {
        response.Ukprn = ukprn;
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = null;
        mediator.Setup(x =>
                x.Send(It.Is<GetProviderQuery>(c =>
                    c.Ukprn.Equals(ukprn)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var actual = await controller.Index(ukprn, location);

        actual.Should().NotBeNull();

        var actualVm = actual as ViewResult;
        actualVm.Should().NotBeNull();

        var model = actualVm!.Model as ProviderDetailsViewModel;
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(response, options => options
            .Excluding(r => r.ProviderAddress)
            .Excluding(r => r.Qar)
            .Excluding(r => r.Reviews)
            .Excluding(r => r.Courses)
            .Excluding(r => r.EndpointAssessments)
            .Excluding(r => r.AnnualApprenticeFeedbackDetails)
            .Excluding(r => r.AnnualEmployerFeedbackDetails)
            .Excluding(r => r.Contact.Website)
        );

        model!.ShowSearchCrumb.Should().Be(true);
        model.ShowShortListLink.Should().Be(true);
        model.Location.Should().Be(location);
    }
}
