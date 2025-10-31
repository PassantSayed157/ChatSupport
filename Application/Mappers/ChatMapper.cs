using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class ChatMapper
    {
        public static ChatSessionDto MapToDto(this ChatSession s)
        {
            return new ChatSessionDto
            {
                Id = s.Id.ToString(),
                UserId = s.UserId,
                AssignedAgentId = s.AssignedAgentId.ToString(),
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt,
                LastPollTime = s.LastPollTime,
                IsOverflow = s.IsOverflow,
                Messages = s.Messages.Select(m => new MessageDto
                {
                    Text = m.Text,
                    IsUser = m.IsUser,
                    SentAt = m.SentAt
                }).ToList()
            };
        }
    }
}
