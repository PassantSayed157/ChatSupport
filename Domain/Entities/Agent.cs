using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Agent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public SeniorityLevel Seniority { get; set; }
        public ShiftType Shift { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int ActiveChats { get; set; } = 0;
        public byte[]? RowVersion { get; set; }

        public double Efficiency => Seniority switch
        {
            SeniorityLevel.Junior => 0.4,
            SeniorityLevel.Mid => 0.6,
            SeniorityLevel.Senior => 0.8,
            SeniorityLevel.Lead => 0.5,
            _ => 0.4
        };

        public int MaxConcurrentChats => (int)Math.Floor(10 * Efficiency);
    }
}
