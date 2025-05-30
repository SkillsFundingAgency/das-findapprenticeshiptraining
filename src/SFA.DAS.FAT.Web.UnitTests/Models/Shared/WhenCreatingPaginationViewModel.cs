﻿using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Infrastructure;
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
    public void Then_Page_Number_Should_Be_Set()
    {
        var sut = new PaginationViewModel(
            1, 
            1, 
            PageSize, 
            _urlHelperMock.Object, 
            RouteNames.Courses, new List<(string, string)>()
        );

        Assert.That(sut.PageNumber, Is.EqualTo(1));
    }

    [Test]
    [InlineAutoData(1, 0, "When no records")]
    [InlineAutoData(1, 10, "When only enough records for one page")]
    [InlineAutoData(3, 10, "When page number is greater than total pages")]
    public void Then_Returns_Empty_Model(int currentPage, int totalCount, string testMessage)
    {
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, new List<(string, string)>());
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

        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, urlHelperMock.Object, RouteNames.Courses, new List<(string, string)>());

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
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, new List<(string, string)>());

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

        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, new List<(string, string)>());

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(5);
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain("PageNumber=1");
            sut.Pages[1].Title.Should().Be("1");
            sut.Pages[1].Url.Should().Contain("PageNumber=1");
            sut.Pages[2].Title.Should().Be("2");
            sut.Pages[2].Url.Should().BeNull();
            sut.Pages[3].Title.Should().Be("3");
            sut.Pages[3].Url.Should().Contain("PageNumber=3");
            sut.Pages[4].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[4].Url.Should().Contain("PageNumber=3");
        }
    }

    [Test]
    public void When_Params_Do_Not_Contain_PageNumber_Then_PageNumber_Is_Added_And_Correct_Page_Links_Are_Set()
    {
        int currentPage = 1;
        int totalCount = 20;

        List<ValueTuple<string, string>> queryParams = [];

        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, queryParams);

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(3);
            sut.Pages[1].Title.Should().Be("2");
            sut.Pages[1].Url.Should().Contain("PageNumber=2");
        }
    }

    [TestCase(1, 10, 0, 0, "When there is only 1 page")]
    [TestCase(2, 11, 2, 1, "When there is only 1 page")]
    [TestCase(3, 70, 6, 1, "When there is only 1 page")]
    [TestCase(4, 70, 6, 2, "When there is only 1 page")]
    [TestCase(9, 90, 6, 4, "When there is only 1 page")]
    public void Then_Model_Creates_Correct_Number_Of_Page_Links(int currentPage, int totalCount, int expectedPageLinksCount, int startPageNumber, string testMessage)
    {
        var sut = new PaginationViewModel(currentPage, totalCount, PageSize, _urlHelperMock.Object, RouteNames.Courses, new List<(string, string)>());

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

    [TestCase(1, 70, 10, 1, 6, TestName = "First Page - Expands End")]
    [TestCase(2, 70, 10, 1, 6, TestName = "Second Page - Expands End")]
    [TestCase(3, 70, 10, 1, 6, TestName = "Third Page - Expands End")]
    [TestCase(5, 70, 10, 2, 7, TestName = "Middle Page - Keeps Balance")]
    [TestCase(6, 70, 10, 2, 7, TestName = "Sixth Page - Expands Start")]
    [TestCase(7, 70, 10, 2, 7, TestName = "Seventh Page - Expands Start")]
    [TestCase(7, 80, 10, 3, 8, TestName = "Eighth Page - Expands Start")]
    [TestCase(20, 100, 10, 5, 10, TestName = "Current Page Exceeds Last Page - Resets Start Page")]
    public void Then_Get_Page_Range_Adjusts_Correctly(
        int currentPage,
        int totalRecords,
        int pageSize,
        int expectedStartPage,
        int expectedEndPage
    )
    {
        var (startPage, endPage) = PaginationViewModel.GetPageRange(currentPage, totalRecords, pageSize);

        Assert.Multiple(() =>
        {
            Assert.That(startPage, Is.EqualTo(expectedStartPage), "Start page did not match expected value.");
            Assert.That(endPage, Is.EqualTo(expectedEndPage), "End page did not match expected value.");
        });
    }

    [Test]
    public void When_Url_Is_Set_Then_Page_Link_Has_Link_Is_True()
    {
        PageLink pageLink = new PageLink("Title", "//dummyurl");
        Assert.That(pageLink.HasLink, Is.True);
    }

    [Test]
    public void When_Url_Is_Not_Set_Then_Page_Link_Has_Link_Is_False()
    {
        PageLink pageLink = new PageLink("Title", string.Empty);
        Assert.That(pageLink.HasLink, Is.False);
    }
}
