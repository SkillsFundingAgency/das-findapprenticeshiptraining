using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Application.Providers.Query.GetProvider;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Providers;
using SFA.DAS.FAT.Web.Validators;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.ProvidersControllerTests;

public class ProvidersControllerTests
{
    [Test, MoqAutoData]
    public async Task Index_WhenProviderExists_ReturnsProviderDetailsView(
        int ukprn,
        GetProviderQueryResponse response,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] ProvidersController controller)
    {
        response.Ukprn = ukprn;
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = null;
        mediator.Setup(x => x.Send(It.Is<GetProviderQuery>(c =>
                 c.Ukprn.Equals(ukprn)), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
           .Returns(new LocationCookieItem { Location = location, Distance = "10" });

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        var actual = await controller.Index(ukprn);

        actual.Should().NotBeNull();

        var actualVm = actual as ViewResult;
        actualVm.Should().NotBeNull();

        var model = actualVm!.Model as ProviderDetailsViewModel;
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(response, options => options
            .ExcludingMissingMembers()
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

    [Test, MoqAutoData]
    public async Task Index_InvalidUkprn_ReturnsNotFound(
        int ukprn,
        GetProviderQueryResponse response,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Greedy] ProvidersController controller)
    {
        response.Ukprn = ukprn;
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = null;
        mediator.Setup(x =>
                x.Send(It.Is<GetProviderQuery>(c =>
                    c.Ukprn.Equals(ukprn)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new("Ukprn", GetCourseProviderDetailsQueryValidator.InvalidUkprnErrorMessage )
            }
        });

        var sut = await controller.Index(ukprn);

        sut.Should().BeOfType<NotFoundResult>();

        mediator.Verify(m => m.Send(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test, MoqAutoData]
    public async Task Index_NullResponse_ReturnsNotFound(
        int ukprn,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Greedy] ProvidersController controller)
    {
        mediator.Setup(x =>
                x.Send(It.Is<GetProviderQuery>(c =>
                    c.Ukprn.Equals(ukprn)), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProviderQueryResponse)null);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        var sut = await controller.Index(ukprn);

        sut.Should().BeOfType<NotFoundResult>();
    }
}
