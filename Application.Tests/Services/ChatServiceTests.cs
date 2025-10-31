using Application.Interfaces;
using Application.Models;
using Application.Services;
using Castle.Core.Logging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.Services
{
    public class ChatServiceTests
    {
        private readonly Mock<IChatRepository> mockChatRepo;
        private readonly Mock<IAgentRepository> mockAgentRepo;
        private readonly Mock<IQueueService> mockQueue;
        private readonly Mock<IAgentAssignmentService> mockAssignmentService;
        private readonly ChatService service;
        private readonly Mock<ILogger<ChatService>> mockLogger;

        public ChatServiceTests()
        {
            mockChatRepo = new Mock<IChatRepository>();
            mockAgentRepo = new Mock<IAgentRepository>();
            mockQueue = new Mock<IQueueService>();
            mockAssignmentService = new Mock<IAgentAssignmentService>();
            mockLogger = new Mock<ILogger<ChatService>>();

            service = new ChatService(
                mockQueue.Object,
                mockAssignmentService.Object,
                mockAgentRepo.Object,
                mockChatRepo.Object,
                mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateAndEnqueueShouldCreateSessionAndQueueAndAssignAgent()
        {
            var request = new ChatRequestDto
            {
                UserId = "user1",
                FirstMessage = "Hi!"
            };

            var agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Name = "Sara", IsAvailable = true }
            };

            mockAgentRepo.Setup(a => a.GetAvailableAgentsAsync())
                          .ReturnsAsync(agents);

            mockAssignmentService.Setup(a => a.SelectAgent(It.IsAny<IList<Agent>>()))
                         .Returns(agents[0]);

            mockAgentRepo.Setup(a => a.TryIncrementActiveChatsAsync(It.IsAny<Guid>()))
                          .ReturnsAsync(true);

            var result = await service.CreateAndEnqueue(request);

            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);
            mockChatRepo.Verify(c => c.AddAsync(It.IsAny<ChatSession>()), Times.Once);
            mockQueue.Verify(q => q.Enqueue(It.IsAny<ChatRequestDto>()), Times.Once);
            mockChatRepo.Verify(c => c.UpdateAsync(It.IsAny<ChatSession>()), Times.Once);
        }

        [Fact]
        public async Task CreateAndEnqueueShouldStayQueuedWhenNoAgentAvailable()
        {
            var request = new ChatRequestDto { UserId = "user2", FirstMessage = "test" };

            mockAgentRepo.Setup(a => a.GetAvailableAgentsAsync())
                          .ReturnsAsync(new List<Agent>());

            mockAssignmentService.Setup(a => a.SelectAgent(It.IsAny<IList<Agent>>()))
                         .Returns((Agent?)null);

            var result = await service.CreateAndEnqueue(request);

            Assert.NotNull(result);
            mockChatRepo.Verify(c => c.UpdateAsync(It.IsAny<ChatSession>()), Times.Never);
        }

        [Fact]
        public async Task PollAsyncShouldUpdateLastPollTimeWhenSessionExists()
        {
            var sessionId = Guid.NewGuid();
            var fakeSession = new ChatSession
            {
                Id = sessionId,
                UserId = "user3",
                LastPollTime = DateTime.Now.AddMinutes(-10)
            };

            mockChatRepo.Setup(r => r.GetByIdAsync(sessionId))
                         .ReturnsAsync(fakeSession);

            await service.PollAsync(sessionId);

            mockChatRepo.Verify(r => r.UpdateAsync(It.Is<ChatSession>(s =>
                s.LastPollTime > DateTime.Now.AddSeconds(-2)
            )), Times.Once);
        }

        [Fact]
        public async Task PollAsyncShouldDoNothingWhenSessionNotFound()
        {
            var sessionId = Guid.NewGuid();

            mockChatRepo.Setup(r => r.GetByIdAsync(sessionId))
                         .ReturnsAsync((ChatSession?)null);

            await service.PollAsync(sessionId);

            mockChatRepo.Verify(r => r.UpdateAsync(It.IsAny<ChatSession>()), Times.Never);
        }
    }
}
