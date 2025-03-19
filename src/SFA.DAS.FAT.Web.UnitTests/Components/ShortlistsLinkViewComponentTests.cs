using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Components;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Shared.Components;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Components;

public class ShortlistsLinkViewComponentTests
{
    [Test, MoqAutoData]
    public async Task When_InvokeAsync_Then_Should_Return_ViewComponentResult_If_Cookie_Found(
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<IShortlistService> shortlistServiceMock,
        ShortlistCookieItem shortlistCookieItem,
        int count,
        [Greedy] ShortlistsLinkViewComponent sut)
    {
        // Arrange
        shortlistCookieServiceMock.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns(shortlistCookieItem);
        shortlistServiceMock.Setup(x => x.GetShortlistsCountForUser(shortlistCookieItem.ShortlistUserId)).ReturnsAsync(count);
        // Act
        var result = await sut.InvokeAsync();
        // Assert
        result.As<ViewViewComponentResult>().ViewData.Model.As<ShortlistsLinkComponentViewModel>().Count.Should().Be(count);
    }

    [Test, MoqAutoData]
    public async Task When_InvokeAsync_Then_Should_Return_ViewComponentResult_With_Zero_Count_If_Cookie_Not_Found(
        [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieServiceMock,
        [Frozen] Mock<IShortlistService> shortlistServiceMock,
        ShortlistCookieItem shortlistCookieItem,
        [Greedy] ShortlistsLinkViewComponent sut)
    {
        // Arrange
        shortlistCookieServiceMock.Setup(x => x.Get(Constants.ShortlistCookieName)).Returns((ShortlistCookieItem)default);
        // Act
        var result = await sut.InvokeAsync();
        // Assert
        result.As<ViewViewComponentResult>().ViewData.Model.As<ShortlistsLinkComponentViewModel>().Count.Should().Be(0);
        shortlistServiceMock.Verify(x => x.GetShortlistsCountForUser(shortlistCookieItem.ShortlistUserId), Times.Never);

    }
}
