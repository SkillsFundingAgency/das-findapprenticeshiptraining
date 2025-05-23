﻿using System.Text;
using System.Xml;
using System.Xml.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Standards.Queries.GetStandards;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.Testing.AutoFixture;


namespace SFA.DAS.FAT.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenGettingTheSitemap
    {
        [Test, MoqAutoData]
        public async Task Then_The_Routes_Are_Added(
            string serviceStartUrl,
            string shortlistUrl,
            string privacyUrl,
            string accessibilityUrl,
            string cookiesUrl,
            string cookiesDetailsUrl,
            string coursesUrl,
            GetStandardsQueryResult result,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IDistributedCache> cache,
            [Frozen] Mock<IConfiguration> configuration)
        {
            var httpContext = new DefaultHttpContext();
            var urlHelper = new Mock<IUrlHelper>();
            cache.Setup(x => x.GetAsync("Sitemap", CancellationToken.None))
                .ReturnsAsync((byte[])default);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ServiceStart))))
                .Returns(serviceStartUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.Courses))))
                .Returns(coursesUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.ShortLists))))
                .Returns(shortlistUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.Privacy))))
                .Returns(privacyUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.AccessibilityStatement))))
                .Returns(accessibilityUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.Cookies))))
                .Returns(cookiesUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.CookiesDetails))))
                .Returns(cookiesDetailsUrl);

            mediator.Setup(x => x.Send(It.IsAny<GetStandardsQuery>(), CancellationToken.None)).ReturnsAsync(result);

            var controller = new HomeController(mediator.Object, cache.Object, configuration.Object)
            {
                Url = urlHelper.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var actual = await controller.SitemapXml() as ContentResult;

            //Assert
            using (new AssertionScope())
            {
                actual.Should().NotBeNull();
                var document = new XmlDocument();
                document.LoadXml(actual!.Content!);

                var newNode = XDocument.Parse(actual.Content).Root;
                newNode.Should().NotBeNull();
                var nodes = newNode.Descendants(newNode.GetDefaultNamespace() + "loc").ToList();
                nodes.Any(c => c.Value.Equals(serviceStartUrl)).Should().BeTrue();
                nodes.Any(c => c.Value.Equals(coursesUrl)).Should().BeTrue();
                nodes.Any(c => c.Value.Equals(shortlistUrl)).Should().BeTrue();
                nodes.Any(c => c.Value.Equals(privacyUrl)).Should().BeTrue();
                nodes.Any(c => c.Value.Equals(accessibilityUrl)).Should().BeTrue();
                nodes.Any(c => c.Value.Equals(cookiesUrl)).Should().BeTrue();
                nodes.Any(c => c.Value.Equals(cookiesDetailsUrl)).Should().BeTrue();
            }
        }

        [Test, MoqAutoData]
        public async Task Then_The_Courses_Query_Is_Called_And_Nodes_Built_For_Each_Course_And_Provider_List(
            string courseUrl,
            string courseProvidersUrl,
            GetStandardsQueryResult result,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IDistributedCache> cache,
            [Frozen] Mock<IConfiguration> configuration)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var urlHelper = new Mock<IUrlHelper>();

            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.CourseDetails))))
                .Returns(courseUrl);
            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName.Equals(RouteNames.CourseProviders))))
                .Returns(courseProvidersUrl);
            cache.Setup(x => x.GetAsync("Sitemap", CancellationToken.None))
                .ReturnsAsync((byte[])default);

            var controller = new HomeController(mediator.Object, cache.Object, configuration.Object)
            {
                Url = urlHelper.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            mediator.Setup(x => x.Send(It.IsAny<GetStandardsQuery>(), CancellationToken.None)).ReturnsAsync(result);

            //Act
            var actual = await controller.SitemapXml() as ContentResult;

            //Assert
            actual.Should().NotBeNull();
            var document = new XmlDocument();
            document.LoadXml(actual.Content);

            var newNode = XDocument.Parse(actual.Content).Root;
            newNode.Should().NotBeNull();
            var nodes = newNode.Descendants(newNode.GetDefaultNamespace() + "loc").ToList();

            nodes.Count(c => c.Value.Equals(courseUrl)).Should().Be(result.Standards.Count);
            nodes.Count(c => c.Value.Equals(courseProvidersUrl)).Should().Be(result.Standards.Count);

        }

        [Test, MoqAutoData]
        public async Task Then_The_Sitemap_Content_Is_Added_To_The_Cache(
            string courseUrl,
            string courseProvidersUrl,
            GetStandardsQueryResult result,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IDistributedCache> cache,
            [Frozen] Mock<IConfiguration> configuration)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var urlHelper = new Mock<IUrlHelper>();
            cache.Setup(x => x.GetAsync("Sitemap", CancellationToken.None))
                .ReturnsAsync((byte[])default);

            mediator.Setup(x => x.Send(It.IsAny<GetStandardsQuery>(), CancellationToken.None)).ReturnsAsync(result);

            var controller = new HomeController(mediator.Object, cache.Object, configuration.Object)
            {
                Url = urlHelper.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            await controller.SitemapXml();

            //Assert
            cache.Verify(x => x.SetAsync("Sitemap", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Cache_Object_Exists_Then_It_Is_Returned(
            GetStandardsQueryResult result,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IDistributedCache> cache,
            [Frozen] Mock<IConfiguration> configuration)
        {
            //Arrange
            var expectedContent = "<xml></xml>";
            var httpContext = new DefaultHttpContext();
            var urlHelper = new Mock<IUrlHelper>();
            cache.Setup(x => x.GetAsync("Sitemap", CancellationToken.None))
                .ReturnsAsync(Encoding.UTF8.GetBytes(expectedContent));
            var controller = new HomeController(mediator.Object, cache.Object, configuration.Object)
            {
                Url = urlHelper.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            //Act
            var actual = await controller.SitemapXml() as ContentResult;

            //Assert
            actual.Should().NotBeNull();
            actual.Content.Should().Be(expectedContent);
            mediator.Verify(x => x.Send(It.IsAny<GetStandardsQuery>(), CancellationToken.None), Times.Never);
        }
    }
}
