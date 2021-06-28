using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Infrastructure;

namespace SFA.DAS.FAT.Web.UnitTests.Infrastructure
{
    public class WhenRedirectingTheRequest
    {
        private RewriteContext _rewriteContext;

        [SetUp]
        public void Arrange()
        {
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?ukprn=1001&standardCode=2&Postcode=coventry");
            context.Request.Path = new PathString("/Provider/Detail");
            context.Request.Host = new HostString("local"); 
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            _rewriteContext = new RewriteContext
            {
                HttpContext = mockHttpContextAccessor.Object.HttpContext
            };
        }
        
        [Test]
        public void Then_If_The_Request_Is_The_Same_As_The_Replacement_No_Action_Is_Taken()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString("/courses");
            context.Request.Host = new HostString("local"); 
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var rewriteContext = new RewriteContext
            {
                HttpContext = mockHttpContextAccessor.Object.HttpContext
            };
            
            var actual = new PathWithQueryRule("courses", "/courses", new List<string>());
            actual.ApplyRule(rewriteContext);

            rewriteContext.HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [Test]
        public void Then_If_The_Request_Does_Not_Match_The_Regex_Pattern_No_Action_Is_Taken()
        {
            //Arrange
            var actual = new PathWithQueryRule("courses", "/courses", new List<string>());
            
            //Act
            actual.ApplyRule(_rewriteContext);

            //Assert
            _rewriteContext.HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [Test]
        public void Then_If_The_Request_Matches_The_Regex_Pattern_Then_The_Request_Is_Redirected()
        {
            //Arrange
            var actual = new PathWithQueryRule("(?i)provider/detail\\b", "/courses", new List<string>());
            
            //Act
            actual.ApplyRule(_rewriteContext);

            //Assert
            _rewriteContext.HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.PermanentRedirect);
            _rewriteContext.HttpContext.Response.Headers[HeaderNames.Location].Should().BeEquivalentTo("/courses");
        }

        [Test]
        public void Then_If_There_Are_Query_Params_These_Are_Matched()
        {
            //Arrange
            var actual = new PathWithQueryRule("(?i)provider/detail\\b", "/courses/$0/providers/$1?location=$2", new List<string>{"standardCode", "ukprn","postcode"});
            
            //Act
            actual.ApplyRule(_rewriteContext);

            //Assert
            _rewriteContext.HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.PermanentRedirect);
            _rewriteContext.HttpContext.Response.Headers[HeaderNames.Location].Should().BeEquivalentTo("/courses/2/providers/1001?location=coventry");
        }

        [Test]
        public void Then_If_The_Query_Param_Is_A_List_It_Is_Matched_And_Repeated()
        {
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?ukprn=1001&standardCode=2&Postcode=coventry&DeliveryModes=1&DeliveryModes=2&DeliveryModes=3");
            context.Request.Path = new PathString("/Provider/Detail");
            context.Request.Host = new HostString("local"); 
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var rewriteContext = new RewriteContext
            {
                HttpContext = mockHttpContextAccessor.Object.HttpContext
            };
            
            var actual = new PathWithQueryRule("(?i)provider/detail\\b", "/courses/$0/providers/$1?location=$2&DeliveryModes=$3", new List<string>{"standardCode", "ukprn","postcode", "deliverymodes"});
            actual.ApplyRule(rewriteContext);

            rewriteContext.HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.PermanentRedirect);
            rewriteContext.HttpContext.Response.Headers[HeaderNames.Location].Should().BeEquivalentTo("/courses/2/providers/1001?location=coventry&DeliveryModes=1&DeliveryModes=2&DeliveryModes=3");
        }

        [Test]
        public void Then_Any_Unmatched_Query_Params_Are_Removed_From_The_Url()
        {
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?ukprn=1001&standardCode=2&Postcode=");
            context.Request.Path = new PathString("/Provider/Detail");
            context.Request.Host = new HostString("local"); 
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var rewriteContext = new RewriteContext
            {
                HttpContext = mockHttpContextAccessor.Object.HttpContext
            };
            
            var actual = new PathWithQueryRule("(?i)provider/detail\\b", "/courses/$0/providers/$1?location=$2&DeliveryModes=$3", new List<string>{"standardCode", "ukprn","postcode", "deliverymodes"});
            actual.ApplyRule(rewriteContext);

            rewriteContext.HttpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.PermanentRedirect);
            rewriteContext.HttpContext.Response.Headers[HeaderNames.Location].Should().BeEquivalentTo("/courses/2/providers/1001?location=&DeliveryModes=");
        }
    }
}