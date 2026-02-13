using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.SearchCoursesControllerTests;

public class WhenGettingSearchCourses
{
    [Test, MoqAutoData]
    public void Index_CookieMissing_BuildsViewModel(
        [Greedy] SearchCoursesController controller)
    {
        var actual = controller.Index() as ViewResult;

        actual.Should().NotBeNull();
        var model = actual!.Model as SearchCoursesViewModel;
        model.Should().NotBeNull();
        model.ShowSearchCrumb.Should().BeFalse();
        model.ShowShortListLink.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public void Index_GetRequest_PopulatesTrainingTypesFilterItemsWithBold(
        [Greedy] SearchCoursesController controller)
    {
        var actual = controller.Index() as ViewResult;

        actual.Should().NotBeNull();
        var model = actual!.Model as SearchCoursesViewModel;
        model.Should().NotBeNull();

        model.TrainingTypesFilterItems.Should().NotBeNull();
        model.TrainingTypesFilterItems.Should().HaveCount(3);
        model.TrainingTypesFilterItems.All(i => i.IsApprenticeshipTypeEmphasised).Should().BeTrue();
    }
}
