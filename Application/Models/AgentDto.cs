using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class AgentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Seniority { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int ActiveChats { get; set; }
        public int MaxConcurrentChats { get; set; }
    }
}
