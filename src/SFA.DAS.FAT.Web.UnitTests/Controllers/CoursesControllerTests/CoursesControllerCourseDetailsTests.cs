using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests;

public class CoursesControllerCourseDetailsTests
{
    [Test]
    [MoqAutoData]
    public async Task WhenGettingCourseDetails_AndRequestIsValid_ThenReturnsValidViewModel(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut
    )
    {
        string larsCode = "123";
        string location = "London";
        string distance = "20";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        mediator
            .Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        var result = await sut.CourseDetails(larsCode) as ViewResult;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);

            var viewModel = result.Model as CourseViewModel;
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Title, Is.EqualTo(queryResult.Title));
            Assert.That(viewModel.LocationName, Is.EqualTo(location));
            Assert.That(viewModel.Distance, Is.EqualTo(distance));
        }
    }

    [Test]
    [MoqAutoData]
    public async Task WhenGettingCourseDetails_AndDistanceFilterIsAcrossEngland_ThenApiIsCalledWithDefaultDistanceValue(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> sut,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController controller
    )
    {
        string larsCode = "123";
        string location = "London";
        string distance = "All";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        sut.Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        await controller.CourseDetails(larsCode);

        sut.Verify(x =>
            x.Send(It.Is<GetCourseQuery>(a =>
                    a.Distance.Equals(DistanceService.AcrossEnglandDistance) &&
                    a.LocationName.Equals(location) &&
                    a.LarsCode.Equals(larsCode)
                ), It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Test]
    [MoqAutoData]
    public async Task WhenGettingCourseDetails_AndDistanceIsInvalid_ThenDistanceDefaultsToTenMiles(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> sut,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController controller
    )
    {
        string larsCode = "123";
        string location = "London";
        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location });

        sut.Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        await controller.CourseDetails(larsCode);

        sut.Verify(x =>
            x.Send(It.Is<GetCourseQuery>(a =>
                    a.Distance.Equals(DistanceService.DefaultDistance) &&
                    a.LocationName.Equals(location) &&
                    a.LarsCode.Equals(larsCode)), It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Test]
    [MoqAutoData]
    public async Task WhenGettingCourseDetails_AndDistanceIsValid_ThenConvertedDistanceIsUsedInRequest(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> sut,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController controller
    )
    {
        string courseId = "123";
        string location = "London";
        string distance = "20";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = location, Distance = distance });

        sut.Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        await controller.CourseDetails(courseId);

        sut.Verify(x =>
            x.Send(It.Is<GetCourseQuery>(a =>
                    a.Distance.Equals(Convert.ToInt32(distance)) &&
                    a.LocationName.Equals(location) &&
                    a.LarsCode.Equals(courseId)
                ), It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseDetails_AndLarsCodeIsEmpty_ThenRedirectsToShutterPage(
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] CoursesController sut
    )
    {
        var larsCode = string.Empty;

        var result = await sut.CourseDetails(larsCode);

        result.Should().BeOfType<NotFoundResult>();
        mediator.Verify(m => m.Send(It.Is<GetCourseQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingCourseDetails_AndLarsCodeIsInvalid_ThenRedirectsToShutterPage(
        string larsCode,
        string location,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] CoursesController sut
    )
    {
        mediator
            .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCourseQueryResult)null);

        var result = await sut.CourseDetails(larsCode);

        result.Should().BeOfType<NotFoundResult>();
        mediator.Verify(m => m.Send(It.Is<GetCourseQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    [MoqAutoData]
    public async Task WhenGettingCourseDetails_AndCourseNotFound_ThenRedirectsTo404(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController sut
    )
    {
        string larsCode = "999";
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediator
            .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCourseQueryResult)null);

        var result = await sut.CourseDetails(larsCode);

        result.Should().BeOfType<NotFoundResult>();
    }
}
