using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public Guid? AssignedAgentId { get; set; }
        public ChatStatus Status { get; set; } = ChatStatus.Queued;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastPollTime { get; set; } = DateTime.Now;
        public bool IsOverflow { get; set; } = false;

        public List<Message> Messages { get; set; } = new();
    }
}
