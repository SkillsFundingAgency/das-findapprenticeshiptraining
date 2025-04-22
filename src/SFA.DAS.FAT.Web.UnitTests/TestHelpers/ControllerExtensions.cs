using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace SFA.DAS.FAT.Web.UnitTests.TestHelpers;
public static class ControllerExtensions
{
    public static Mock<IUrlHelper> AddUrlHelperMock(this Controller controller)
    {
        var urlHelperMock = new Mock<IUrlHelper>();
        controller.Url = urlHelperMock.Object;
        return urlHelperMock;
    }

    public static Mock<IUrlHelper> AddUrlForRoute(this Mock<IUrlHelper> urlHelperMock, string routeName, string url = TestConstants.DefaultUrl)
    {
        urlHelperMock
            .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName!.Equals(routeName))))
            .Returns(url);
        return urlHelperMock;
    }

    static TController AddUrlForRoute<TController>(this TController controller, string routeName, string returnValue)
        where TController : Controller
    {
        var urlHelper = Mock.Get(controller.Url);
        urlHelper.Setup(x => x.RouteUrl(It.Is<UrlRouteContext>(ctx => ctx.RouteName == routeName)))
            .Returns(returnValue);
        return controller;
    }
}
