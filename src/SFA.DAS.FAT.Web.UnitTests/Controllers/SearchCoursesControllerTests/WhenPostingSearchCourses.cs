using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.SearchCoursesControllerTests;
public class WhenPostingSearchCourses
{
    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_No_RouteValues_Set(
        [Greedy] SearchCoursesController controller,
        CancellationToken cancellationToken)
    {
        //Arrange
        SearchCoursesViewModel viewModel = new SearchCoursesViewModel();
        //Act
        var actual = controller.Index(viewModel, cancellationToken);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToActionResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Courses");
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Location");
            result.RouteValues!["Location"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_CourseTerm_In_RouteValues(
        string courseTerm,
        [Greedy] SearchCoursesController controller,
        CancellationToken cancellationToken)
    {
        //Arrange
        SearchCoursesViewModel viewModel = new SearchCoursesViewModel { CourseTerm = courseTerm };
        //Act
        var actual = controller.Index(viewModel, cancellationToken);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToActionResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Courses");
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().Be(courseTerm);
            result.RouteValues.Should().ContainKey("Location");
            result.RouteValues!["Location"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_Location_And_Distance_In_RouteValues(
        string location,
        [Greedy] SearchCoursesController controller,
        CancellationToken cancellationToken)
    {
        //Arrange
        SearchCoursesViewModel viewModel = new SearchCoursesViewModel { Location = location };
        //Act
        var actual = controller.Index(viewModel, cancellationToken);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToActionResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Courses");
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Location");
            result.RouteValues!["Location"].Should().Be(location);
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().Be(DistanceService.TEN_MILES.ToString());
        }
    }

    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_Location_And_Course_Term_In_Route_Values(
        string location,
        string courseTerm,
        [Greedy] SearchCoursesController controller,
        CancellationToken cancellationToken)
    {
        //Arrange
        SearchCoursesViewModel viewModel = new SearchCoursesViewModel { Location = location, CourseTerm = courseTerm };
        //Act
        var actual = controller.Index(viewModel, cancellationToken);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToActionResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Courses");
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().Be(courseTerm);
            result.RouteValues.Should().ContainKey("Location");
            result.RouteValues!["Location"].Should().Be(location);
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().Be(DistanceService.TEN_MILES.ToString());
        }
    }
}
