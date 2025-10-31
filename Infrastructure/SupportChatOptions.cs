using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class SupportChatOptions
    {
        public int PollIntervalSeconds { get; set; }
        public int MaxMissedPolls { get; set; }
        public int OverflowCount { get; set; }
        public string OfficeHoursStart { get; set; } = string.Empty;
        public string OfficeHoursEnd { get; set; } = string.Empty;
    }
}
