using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FCG.Application.Web;
using FCG.WebApi.Controllers;
using Moq;
using Xunit;

namespace FCG.Test
{
    // Helper to expose protected TryMethodAsync
    public class TestController : StandardController
    {
        public Task<IActionResult> CallTryMethodAsync<TResult>(Func<Task<IApiResponse<TResult>>> serviceMethod, ILogger logger)
            => TryMethodAsync(serviceMethod, logger);
    }

    public class StandardControllerTests
    {
        private readonly TestController _controller;
        private readonly Mock<ILogger> _logger = new();

        public StandardControllerTests()
        {
            _controller = new TestController();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task TryMethodAsync_NoContent_Returns204()
        {
            Task<IApiResponse<bool>> Service() => Task.FromResult(FCG.Application.Services.BaseService.NoContent());
            var result = await _controller.CallTryMethodAsync(Service, _logger.Object);

            // O resultado para NoContent é um StatusCodeResult (204) — verificamos o StatusCode.
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, statusResult.StatusCode);
        }

        [Fact]
        public async Task TryMethodAsync_ServiceThrowsValidationException_ReturnsBadRequestProblemDetails()
        {
            Task<IApiResponse<int>> Service() => throw new System.ComponentModel.DataAnnotations.ValidationException("invalid");
            var result = await _controller.CallTryMethodAsync(Service, _logger.Object);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.IsType<ProblemDetails>(objectResult.Value);
        }
    }
}