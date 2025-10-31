using Application.Interfaces;
using Application.Mappers;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IQueueService queueService;
        private readonly IAgentAssignmentService agentAssignmentService;
        private readonly IAgentRepository agentRepository;
        private readonly IChatRepository chatRepository;
        private readonly ILogger<ChatService> logger;

        public ChatService(
            IQueueService _queueService,
            IAgentAssignmentService _agentAssignmentService,
            IAgentRepository _agentRepository,
            IChatRepository _chatRepository,
            ILogger<ChatService> _logger)
        {
            queueService = _queueService;
            agentAssignmentService = _agentAssignmentService;
            agentRepository = _agentRepository;
            chatRepository = _chatRepository;
            logger = _logger;
        }

        public async Task<ChatSessionDto> CreateAndEnqueue(ChatRequestDto request)
        {
            logger.LogInformation("Create Session started");
            var session = new ChatSession
            {
                UserId = request.UserId,
                CreatedAt = DateTime.Now,
                LastPollTime = DateTime.Now,
                Status = ChatStatus.Queued,
                Messages = new List<Message>
                {
                    new Message { Text = request.FirstMessage, IsUser = true }
                }
            };

            await chatRepository.AddAsync(session);

            logger.LogInformation("Session created successful");

            queueService.Enqueue(request);

            logger.LogInformation("Enqueue Session finished");

            var availableAgents = await agentRepository.GetAvailableAgentsAsync();

            var selected = agentAssignmentService.SelectAgent(availableAgents);

            if (selected != null && await agentRepository.TryIncrementActiveChatsAsync(selected.Id))
            {
                session.AssignedAgentId = selected.Id;
                session.Status = ChatStatus.Active;

                await chatRepository.UpdateAsync(session);
                logger.LogInformation("Session updated finished");
            }

            return session.MapToDto();
        }

        public async Task PollAsync(Guid sessionId)
        {
            logger.LogInformation("Poll Session started with id {@sessionId}", sessionId);

            var session = await chatRepository.GetByIdAsync(sessionId);

            if (session == null) return;

            session.LastPollTime = DateTime.Now;
            await chatRepository.UpdateAsync(session);
        }

        public async Task<IEnumerable<ChatSessionDto>> GetAllSessionsAsync()
        {
            var sessions = await chatRepository.GetAllAsync();
            return sessions.Select(s => s.MapToDto());
        }

        public async Task<ChatSessionDto?> GetByIdAsync(Guid sessionId)
        {
            var session = await chatRepository.GetByIdAsync(sessionId);
            return session != null ? session.MapToDto() : null;
        }
    }
}
