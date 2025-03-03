using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Shared;

public sealed class WhenCreatingPaginationViewModel
{
    private const int PageSize = 10;

    private Mock<IUrlHelper> _urlHelperMock;

    [SetUp]
    public void SetUp()
    {
        _urlHelperMock = new Mock<IUrlHelper>();
        _urlHelperMock
            .Setup(x =>
                x.RouteUrl(
                    It.Is<UrlRouteContext>(c =>
                        c.RouteName!.Equals(RouteNames.Courses)
                    )
                )
            )
        .Returns(TestConstants.DefaultUrl);
    }

    [Test]
    [InlineAutoData(1, 0, "When no records")]
    [InlineAutoData(1, 10, "When only enough records for one page")]
    [InlineAutoData(3, 10, "When page number is greater than total pages")]
    public void Then_Returns_Empty_Model(int currentPage, int totalCount, string testMessage)
    {
        List<ValueTuple<string, string>> queryParams = new() { ValueTuple.Create(nameof(CoursesViewModel.PageNumber), currentPage.ToString()) };
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, queryParams);
        sut.Pages.Count.Should().Be(0, testMessage);
    }

    [Test]
    [InlineAutoData(1, 0, 0, "Previous link should not exist when there are no records")]
    [InlineAutoData(1, 10, 0, "Previous link should not exist when one page only")]
    [InlineAutoData(1, 70, 0, "Previous link should not exist when on first page with more pages")]
    [InlineAutoData(2, 20, 1, "Previous link should point to previous to current page when current page > 1 and total pages > 1")]
    [InlineAutoData(7, 70, 6, "Previous link should point to previous to current page when on last page and total pages > 1")]
    public void Then_Model_Adds_Previous_Link(int currentPage, int totalCount, int expectedPageNumberInPreviousLink, string testMessage)
    {
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

        List<ValueTuple<string, string>> queryParams = new() { ValueTuple.Create(nameof(CoursesViewModel.PageNumber), currentPage.ToString()) };
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, urlHelperMock.Object, RouteNames.Courses, queryParams);

        if (expectedPageNumberInPreviousLink > 0)
        {
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain($"PageNumber={expectedPageNumberInPreviousLink}");
        }
        else
        {
            sut.Pages.Should().NotContain(p => p.Title == PaginationViewModel.PreviousPageTitle);
        }
    }

    [Test]
    [InlineAutoData(1, 0, 0, "Next link should not exist when no records")]
    [InlineAutoData(1, 10, 0, "Next link should not exist when one page only")]
    [InlineAutoData(2, 20, 0, "Next link should not exist when on last page")]
    [InlineAutoData(1, 20, 2, "Next link should point to current page + 1 when current page is less than total pages")]
    public void Then_Model_Adds_Next_Link(int currentPage, int totalCount, int expectedPageNumberInTheNextLink, string testMessage)
    {
        List<ValueTuple<string, string>> queryParams = new() { ValueTuple.Create(nameof(CoursesViewModel.PageNumber), currentPage.ToString()) };
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, queryParams);

        if (expectedPageNumberInTheNextLink > 0)
        {
            var lastPage = sut.Pages.Count - 1;
            sut.Pages[lastPage].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[lastPage].Url.Should().Contain($"PageNumber={expectedPageNumberInTheNextLink}");
        }
        else
        {
            sut.Pages.Find(p => p.Title == PaginationViewModel.NextPageTitle).Should().BeNull();
        }
    }

    [Test]
    public void Then_Model_Sets_Correct_Page_Links()
    {
        int currentPage = 2;
        int totalCount = 30;

        List<ValueTuple<string, string>> queryParams = new() { ValueTuple.Create(nameof(CoursesViewModel.PageNumber), currentPage.ToString()) };

        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, queryParams);

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(5);
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain($"PageNumber={1}");
            sut.Pages[1].Title.Should().Be("1");
            sut.Pages[1].Url.Should().Contain($"PageNumber={1}");
            sut.Pages[2].Title.Should().Be("2");
            sut.Pages[2].Url.Should().BeNull();
            sut.Pages[3].Title.Should().Be("3");
            sut.Pages[3].Url.Should().Contain($"PageNumber={3}");
            sut.Pages[4].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[4].Url.Should().Contain($"PageNumber={3}");
        }
    }


    [TestCase(1, 10, 0, 0, "When there is only 1 page")]
    [TestCase(2, 11, 2, 1, "When there is only 1 page")]
    [TestCase(3, 70, 6, 1, "When there is only 1 page")]
    [TestCase(4, 70, 6, 2, "When there is only 1 page")]
    [TestCase(9, 90, 6, 4, "When there is only 1 page")]
    public void Then_Model_Creates_Correct_Number_Of_Page_Links(int currentPage, int totalCount, int expectedPageLinksCount, int startPageNumber, string testMessage)
    {
        List<ValueTuple<string, string>> queryParams = new() { ValueTuple.Create(nameof(CoursesViewModel.PageNumber), currentPage.ToString()) };
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, queryParams);

        sut.Pages.RemoveAll(p => p.Title == PaginationViewModel.PreviousPageTitle || p.Title == PaginationViewModel.NextPageTitle);

        using (new AssertionScope(testMessage))
        {
            sut.Pages.Should().HaveCount(expectedPageLinksCount, $"Expected page links count: {expectedPageLinksCount}");
            if (expectedPageLinksCount > 0)
            {
                var pageNumber = startPageNumber;
                for (int index = 0; index < expectedPageLinksCount; index++)
                {
                    sut.Pages[index].Title.Should().Be(pageNumber.ToString(), $"Expected page title at index:{index}");
                    pageNumber++;
                }
            }
        }
    }
}
