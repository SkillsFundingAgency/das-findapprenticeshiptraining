using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.FAT.Web.Validators;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CourseProvidersControllerTests;

public class WhenGettingCourseProviderDetails
{
    [Test, MoqAutoData]
    public async Task Then_Mediator_Is_Called_With_The_Correct_Properties(
        int courseId,
        int providerId,
        string location,
        GetCourseProviderQueryResult response,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Greedy] CourseProvidersController sut
    )
    {
        var distance = 10;
        response.AnnualApprenticeFeedbackDetails = new List<ApprenticeFeedbackAnnualSummaries>();
        response.AnnualEmployerFeedbackDetails = new List<EmployerFeedbackAnnualSummaries>();

        shortlistCookieServiceMock.Setup(x =>
            x.Get(Constants.ShortlistCookieName))
        .Returns(shortlistCookieItem);

        mediator.Setup(x => x.Send(It.Is<GetCourseProviderDetailsQuery>(c =>
                c.Ukprn.Equals(providerId) &&
                c.LarsCode.Equals(courseId) &&
                c.Location.Equals(location) &&
                c.Distance.Equals(DistanceService.DEFAULT_DISTANCE) &&
                c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(response);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviderDetails(courseId, providerId, location, distance.ToString());

        Assert.That(result, Is.Not.Null);

        mediator.Verify(a =>
             a.Send(
                 It.Is<GetCourseProviderDetailsQuery>(q =>
                     q.LarsCode == courseId &&
                     q.Ukprn == providerId &&
                     q.Location == location &&
                     q.Distance == DistanceService.DEFAULT_DISTANCE &&
                     q.ShortlistUserId == shortlistCookieItem.ShortlistUserId
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
         );
    }

    [Test, MoqAutoData]
    public async Task When_Provider_Details_Are_Found_Then_Values_Are_Mapped_To_Model_Correctly(
        int courseId,
        int providerId,
        string location,
        GetCourseProviderQueryResult response,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> validatorLocationMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Greedy] CourseProvidersController sut
    )
    {
        var distance = 10;
        response.AnnualApprenticeFeedbackDetails = new List<ApprenticeFeedbackAnnualSummaries>();
        response.AnnualEmployerFeedbackDetails = new List<EmployerFeedbackAnnualSummaries>();

        shortlistCookieServiceMock.Setup(x =>
            x.Get(Constants.ShortlistCookieName))
        .Returns(shortlistCookieItem);

        mediator.Setup(x => x.Send(It.Is<GetCourseProviderDetailsQuery>(c =>
                c.Ukprn.Equals(providerId) &&
                c.LarsCode.Equals(courseId) &&
                c.Location.Equals(location) &&
                c.Distance.Equals(DistanceService.DEFAULT_DISTANCE) &&
                c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(response);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        validatorLocationMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviderDetails(courseId, providerId, location, distance.ToString());

        Assert.That(result, Is.Not.Null);

        mediator.Verify(a =>
             a.Send(
                 It.Is<GetCourseProviderDetailsQuery>(q =>
                     q.LarsCode == courseId &&
                     q.Ukprn == providerId &&
                     q.Location == location &&
                     q.Distance == DistanceService.DEFAULT_DISTANCE &&
                     q.ShortlistUserId == shortlistCookieItem.ShortlistUserId
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
        );

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;

        var model = viewResult.Model.Should().BeOfType<CourseProviderViewModel>().Subject;

        Assert.Multiple(() =>
        {
            Assert.That(model.Ukprn, Is.EqualTo(response.Ukprn));
            Assert.That(model.ProviderName, Is.EqualTo(response.ProviderName));
            Assert.That(model.ProviderAddress, Is.EqualTo(response.ProviderAddress));
            Assert.That(model.Contact, Is.EqualTo(response.Contact));
            Assert.That(model.CourseName, Is.EqualTo(response.CourseName));
            Assert.That(model.Level, Is.EqualTo(response.Level));
            Assert.That(model.LarsCode, Is.EqualTo(response.LarsCode));
            Assert.That(model.IFateReferenceNumber, Is.EqualTo(response.IFateReferenceNumber));
            Assert.That(model.Qar, Is.EqualTo(response.Qar));
            Assert.That(model.Reviews, Is.EqualTo(response.Reviews));
            Assert.That(model.EndpointAssessments, Is.EqualTo(response.EndpointAssessments));
            Assert.That(model.TotalProvidersCount, Is.EqualTo(response.TotalProvidersCount));
            Assert.That(model.ShortlistId, Is.EqualTo(response.ShortlistId));
            Assert.That(model.Locations, Is.EqualTo(response.Locations));
            Assert.That(model.Courses, Is.EqualTo(response.Courses));
            Assert.That(model.CourseId, Is.EqualTo(courseId));
            Assert.That(model.Location, Is.EqualTo(location));
            Assert.That(model.Distance, Is.EqualTo(distance.ToString()));
            Assert.That(model.ShowApprenticeTrainingCourseProvidersCrumb, Is.True);
            Assert.That(model.ShowApprenticeTrainingCourseCrumb, Is.True);
            Assert.That(model.ShowApprenticeTrainingCoursesCrumb, Is.True);
            Assert.That(model.ShowShortListLink, Is.True);
            Assert.That(model.ShowSearchCrumb, Is.True);
        });
    }

    [Test, MoqAutoData]
    public async Task When_Location_Is_Set_Then_Distance_Defaults_To_One_Thousand_Miles(
        int courseId,
        int providerId,
        string location,
        GetCourseProviderQueryResult response,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<ISessionService> sessionService,
        [Greedy] CourseProvidersController sut
    )
    {
        string distance = null;
        response.AnnualApprenticeFeedbackDetails = new List<ApprenticeFeedbackAnnualSummaries>();
        response.AnnualEmployerFeedbackDetails = new List<EmployerFeedbackAnnualSummaries>();

        shortlistCookieServiceMock.Setup(x =>
            x.Get(Constants.ShortlistCookieName))
        .Returns(shortlistCookieItem);

        mediator.Setup(x => x.Send(It.Is<GetCourseProviderDetailsQuery>(c =>
                c.Ukprn.Equals(providerId) &&
                c.LarsCode.Equals(courseId) &&
                c.Location.Equals(location) &&
                c.Distance.Equals(DistanceService.DEFAULT_DISTANCE) &&
                c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(response);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviderDetails(courseId, providerId, location, distance);

        Assert.That(result, Is.Not.Null);

        mediator.Verify(a =>
             a.Send(
                 It.Is<GetCourseProviderDetailsQuery>(q =>
                     q.LarsCode == courseId &&
                     q.Ukprn == providerId &&
                     q.Location == location &&
                     q.Distance == DistanceService.DEFAULT_DISTANCE &&
                     q.ShortlistUserId == shortlistCookieItem.ShortlistUserId
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
         );
    }

    [Test, MoqAutoData]
    public async Task When_Distance_Is_Across_England_Then_Distance_Defaults_To_Null(
        int courseId,
        int providerId,
        string location,
        GetCourseProviderQueryResult response,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<ISessionService> sessionService,
        [Greedy] CourseProvidersController sut
    )
    {
        var distance = "All";
        response.AnnualApprenticeFeedbackDetails = new List<ApprenticeFeedbackAnnualSummaries>();
        response.AnnualEmployerFeedbackDetails = new List<EmployerFeedbackAnnualSummaries>();

        shortlistCookieServiceMock.Setup(x =>
            x.Get(Constants.ShortlistCookieName))
        .Returns(shortlistCookieItem);

        mediator.Setup(x => x.Send(It.Is<GetCourseProviderDetailsQuery>(c =>
                c.Ukprn.Equals(providerId) &&
                c.LarsCode.Equals(courseId) &&
                c.Location.Equals(location) &&
                c.Distance.Equals(DistanceService.DEFAULT_DISTANCE) &&
                c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(response);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        var result = await sut.CourseProviderDetails(courseId, providerId, location, distance);

        var viewResult = result as ViewResult;
        Assert.That(viewResult, Is.Not.Null);

        var model = viewResult.Model as CourseProviderViewModel;
        Assert.That(model, Is.Not.Null);

        mediator.Verify(a =>
             a.Send(
                 It.Is<GetCourseProviderDetailsQuery>(q =>
                     q.LarsCode == courseId &&
                     q.Ukprn == providerId &&
                     q.Location == location &&
                     q.Distance == DistanceService.DEFAULT_DISTANCE &&
                     q.ShortlistUserId == shortlistCookieItem.ShortlistUserId
                 ),
                 It.IsAny<CancellationToken>()
             ),
             Times.Once
         );

        Assert.That(model.Distance, Is.EqualTo(distance));
    }

    [Test, MoqAutoData]
    public async Task When_Provider_Id_Validation_Fails_Then_Redirect_To_Error_Page(
        GetCourseProviderDetailsQuery query,
        GetCourseProviderQueryResult response,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Greedy] CourseProvidersController sut
    )
    {
        var distance = 10;
        var Ukprn = -1;

        shortlistCookieServiceMock.Setup(x =>
            x.Get(Constants.ShortlistCookieName))
        .Returns(shortlistCookieItem);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new(nameof(Ukprn), "Ukprn is invalid")
            }
        });

        var result = await sut.CourseProviderDetails(query.LarsCode, query.Ukprn, query.Location, distance.ToString());

        result.Should().BeOfType<RedirectToRouteResult>();
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.Error404);

        mediator.Verify(m => m.Send(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test, MoqAutoData]
    public async Task When_Location_Validation_Fails_Then_Validation_Message_Shown(
        int courseId,
        int providerId,
        string location,
        GetCourseProviderDetailsQuery query,
        GetCourseProviderQueryResult response,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<IValidator<GetCourseLocationQuery>> validatorLocationMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Greedy] CourseProvidersController sut
    )
    {
        var distance = 10;

        shortlistCookieServiceMock.Setup(x =>
                x.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookieItem);

        validatorMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseProviderDetailsQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult());

        validatorLocationMock.Setup(v =>
            v.ValidateAsync(
                It.IsAny<GetCourseLocationQuery>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new("Location", GetCourseLocationQueryValidator.LocationErrorMessage)
            }
        });

        var result = await sut.CourseProviderDetails(courseId, providerId, location, distance.ToString());

        Assert.That(result, Is.Not.Null);

        mediator.Verify(a =>
                a.Send(
                    It.Is<GetCourseProviderDetailsQuery>(q =>
                        q.LarsCode == courseId &&
                        q.Ukprn == providerId &&
                        q.Location == location &&
                        q.Distance == DistanceService.DEFAULT_DISTANCE &&
                        q.ShortlistUserId == shortlistCookieItem.ShortlistUserId
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();

        var model = viewResult!.Model as CourseProviderViewModel;
        model.Should().NotBeNull();
        model!.LocationEntryError.Should().BeTrue();
        model.Location.Should().Be(string.Empty);
    }

    [Test, MoqAutoData]
    public async Task When_Response_Is_Null_Then_Result_Is_Redirect_To_Error_Page(
        GetCourseProviderDetailsQuery query,
        ShortlistCookieItem shortlistCookieItem,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseProviderDetailsQuery>> validatorMock,
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Greedy] CourseProvidersController sut
    )
    {
        int? distance = 10;

        shortlistCookieServiceMock
            .Setup(x => x.Get(Constants.ShortlistCookieName))
            .Returns(shortlistCookieItem);

        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseProviderDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediator
            .Setup(x => x.Send(It.IsAny<GetCourseProviderDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCourseProviderQueryResult)null);

        var result = await sut.CourseProviderDetails(query.LarsCode, query.Ukprn, query.Location, distance.Value.ToString());

        result.Should().BeOfType<RedirectToRouteResult>();

        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.Error404);

        mediator.Verify(m => m.Send(
            It.Is<GetCourseProviderDetailsQuery>(q =>
                q.LarsCode == query.LarsCode &&
                q.Ukprn == query.Ukprn &&
                q.Location == query.Location &&
                q.Distance == DistanceService.DEFAULT_DISTANCE &&
                q.ShortlistUserId == shortlistCookieItem.ShortlistUserId
            ),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
