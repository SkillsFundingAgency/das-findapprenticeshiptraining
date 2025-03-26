using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Queries.GetShortlistForUser;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.SearchCoursesControllerTests;

public class WhenGettingSearchCourses
{
    [Test, MoqAutoData]
    public void And_Cookie_Does_Not_Exist_Then_Builds_ViewModel(
        GetShortlistForUserResult resultFromMediator,
        Domain.Shortlist.ShortlistForUser shortlistFromService,
        [Greedy] SearchCoursesController controller)
    {

        //Act
        var actual = controller.Index() as ViewResult;

        //Assert
        actual.Should().NotBeNull();
        var model = actual!.Model as SearchCoursesViewModel;
        model.Should().NotBeNull();
        model.ShowSearchCrumb.Should().BeFalse();
        model.ShowShortListLink.Should().BeTrue();
    }
}
