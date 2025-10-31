using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public List<Agent> Agents { get; set; } = new();
        public double Capacity => Agents.Sum(a => a.MaxConcurrentChats);
        public int MaxQueue => (int)Math.Floor(Capacity * 1.5);
    }
}
