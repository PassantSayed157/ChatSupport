using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ChatSessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? AssignedAgentId { get; set; }
        public string Status { get; set; } = "Queued";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastPollTime { get; set; } = DateTime.Now;
        public bool IsOverflow { get; set; } = false;
        public List<MessageDto> Messages { get; set; } = new();
    }
}
