using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AgentAssignmentService : IAgentAssignmentService
    {
        private readonly Dictionary<string, int> roundRobinDictionary = new();
        private readonly string[] preferenceOrder = new[] { "Junior", "Mid", "Senior", "Lead" };

        public Agent? SelectAgent(IList<Agent> availableAgents)
        {
            if (availableAgents.Count == 0) return null;

            var grouped = availableAgents.GroupBy(a => a.Seniority.ToString())
                                         .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var level in preferenceOrder)
            {
                if (!grouped.ContainsKey(level) || grouped[level].Count == 0)
                    continue;

                if (!roundRobinDictionary.ContainsKey(level))
                    roundRobinDictionary[level] = 0;

                var list = grouped[level];
                var index = roundRobinDictionary[level] % list.Count;
                roundRobinDictionary[level] = (roundRobinDictionary[level] + 1) % list.Count;

                return list[index];
            }

            return null;
        }
    }
}
