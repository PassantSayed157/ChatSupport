using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAgentRepository
    {
        Task<List<Agent>> GetAvailableAgentsAsync();
        Task<bool> TryIncrementActiveChatsAsync(Guid agentId);
        Task DecrementActiveChatsAsync(Guid agentId);
        Task UpdateAsync(Agent agent);
    }
}
