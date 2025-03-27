using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests;

public class WhenGettingCourseDetails
{
    [Test]
    [MoqAutoData]
    public async Task And_Valid_Request_Then_Course_Details_Returns_Valid_View_Model(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController sut
    )
    {
        int courseId = 123;
        string location = "London";
        string distance = "20";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        mediator
            .Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        var result = await sut.CourseDetails(courseId, location, distance) as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);

            var viewModel = result.Model as CourseViewModelv2;
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Title, Is.EqualTo(queryResult.Title));
            Assert.That(viewModel.Location, Is.EqualTo(location));
            Assert.That(viewModel.Distance, Is.EqualTo(distance));
        });
    }

    [Test]
    [MoqAutoData]
    public async Task And_Distance_Filter_Is_Across_England_Then_API_Is_Called_With_Default_Distance_Value(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> sut,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController controller
    )
    {
        int courseId = 123;
        string location = "London";
        string distance = "All";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        sut.Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        await controller.CourseDetails(courseId, location, distance);

        sut.Verify(x =>
            x.Send(It.Is<GetCourseQuery>(a =>
                    a.Distance.Equals(DistanceService.DEFAULT_DISTANCE) &&
                    a.Location.Equals(location) &&
                    a.LarsCode.Equals(courseId)
                ), It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Test]
    [MoqAutoData]
    public async Task And_Distance_Is_Invalid_Then_Distance_Defaults_To_Ten_Miles(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> sut,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController controller
    )
    {
        int courseId = 123;
        string location = "London";
        string distance = "-20";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        sut.Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        await controller.CourseDetails(courseId, location, distance);

        sut.Verify(x =>
            x.Send(It.Is<GetCourseQuery>(a =>
                    a.Distance.Equals(DistanceService.TEN_MILES) &&
                    a.Location.Equals(location) &&
                    a.LarsCode.Equals(courseId)
                ), It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Test]
    [MoqAutoData]
    public async Task And_Distance_Is_Valid_Then_Converted_Distance_Is_Used_In_Request(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> sut,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController controller
    )
    {
        int courseId = 123;
        string location = "London";
        string distance = "20";

        validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        sut.Setup(m => m.Send(
                It.IsAny<GetCourseQuery>(),
                It.IsAny<CancellationToken>()
            )
        )
        .ReturnsAsync(queryResult);

        await controller.CourseDetails(courseId, location, distance);

        sut.Verify(x =>
            x.Send(It.Is<GetCourseQuery>(a =>
                    a.Distance.Equals(Convert.ToInt32(distance)) &&
                    a.Location.Equals(location) &&
                    a.LarsCode.Equals(courseId)
                ), It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Test]
    [MoqAutoData]
    public void When_Lars_Code_Is_Invalid_Then_Throws_Validation_Exception(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController sut
    )
    {
        int courseId = 789;
        string location = "SW1";
        string distance = "30";

        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Distance", "Invalid distance provided")
            }));

        Func<Task> act = async () => await sut.CourseDetails(courseId, location, distance);

        act
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("Invalid distance provided");
    }

    [Test]
    [MoqAutoData]
    public async Task When_Course_Not_Found_Then_Redirects_To_404(
        GetCourseQueryResult queryResult,
        [Frozen] Mock<IMediator> mediator,
        [Frozen] Mock<IValidator<GetCourseQuery>> validator,
        [Greedy] CoursesController sut
    )
    {
        int courseId = 999;
        string location = "SW1";
        string distance = "20";

        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediator
            .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCourseQueryResult)null);

        var result = await sut.CourseDetails(courseId, location, distance) as RedirectToRouteResult;

        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.Error404);
    }
}
