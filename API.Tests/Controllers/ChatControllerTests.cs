using API.Controllers;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests.Controllers
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatService> mockChatService;
        private readonly ILogger<ChatController> logger;
        private readonly ChatController controller;

        public ChatControllerTests()
        {
            mockChatService = new Mock<IChatService>();
            var mockLogger = new Mock<ILogger<ChatController>>();
            logger = mockLogger.Object;
            controller = new ChatController(mockChatService.Object, logger);
        }

        [Fact]
        public async Task CreateShouldReturnBadRequestWhenRequestInvalid()
        {
            var invalidRequest = new ChatRequestDto
            {
                UserId = "",
                FirstMessage = ""
            };

            var result = await controller.Create(invalidRequest);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid chat request", badResult.Value);
        }

        [Fact]
        public async Task CreateShouldReturnOkWhenRequestValid()
        {
            var request = new ChatRequestDto
            {
                UserId = "user-1",
                FirstMessage = "Hello"
            };

            var fakeSessionDto = new ChatSessionDto
            {
                UserId = "user-1",
                Messages = new()
            };

            mockChatService.Setup(s => s.CreateAndEnqueue(It.IsAny<ChatRequestDto>()))
                           .ReturnsAsync(fakeSessionDto);

            var result = await controller.Create(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var session = Assert.IsType<ChatSessionDto>(okResult.Value);
            Assert.Equal("user-1", session.UserId);

            mockChatService.Verify(s => s.CreateAndEnqueue(It.IsAny<ChatRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task PollShouldReturnOkWhenCalled()
        {
            var sessionId = Guid.NewGuid();

            mockChatService.Setup(s => s.PollAsync(sessionId))
                            .Returns(Task.CompletedTask);

            var result = await controller.Poll(sessionId);

            Assert.IsType<OkResult>(result);
            mockChatService.Verify(s => s.PollAsync(sessionId), Times.Once);
        }
    }
}
