using AutoFixture.NUnit4;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers;


[TestFixture]
public class ErrorControllerTests
{
    [Test]
    [MoqInlineAutoData(StatusCodes.Status404NotFound, ErrorController.PageNotFoundViewName)]
    [MoqInlineAutoData(StatusCodes.Status500InternalServerError, ErrorController.ErrorInServiceViewName)]
    public void HttpStatusCodeHandler_StatusCode_ReturnsExpectedView(
        int statusCode,
        string expectedViewName,
        [Greedy] ErrorController sut)
    {
        var result = sut.HttpStatusCodeHandler(statusCode) as ViewResult;

        result.Should().NotBeNull();
        result!.ViewName.Should().Be(expectedViewName);
    }

    [Test, MoqAutoData]
    public void ErrorInService_ExceptionFeaturePresent_LogsErrorAndReturnsDefaultView(
       [Greedy] ErrorController sut)
    {
        var context = new DefaultHttpContext();
        var ex = new InvalidOperationException("test failure");
        var feature = new ExceptionHandlerFeature { Error = ex, Path = "/some/path" };
        context.Features.Set<IExceptionHandlerPathFeature>(feature);
        sut.ControllerContext = new ControllerContext { HttpContext = context };

        var result = sut.ErrorInService() as ViewResult;

        result.Should().NotBeNull();
        result!.ViewName.Should().BeNullOrEmpty();
        result!.ViewName.Should().Be("ErrorInService");
    }
}
