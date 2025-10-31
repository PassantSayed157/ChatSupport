using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly EFContext _context;

        public AgentRepository(EFContext context)
        {
            _context = context;
        }

        public async Task<List<Agent>> GetAvailableAgentsAsync()
        {
            var agents = await _context.Agents
                .Where(a => a.IsAvailable)
                .ToListAsync();

            return agents
                .Where(a => a.ActiveChats < a.MaxConcurrentChats)
                .ToList();
        }

        public async Task<bool> TryIncrementActiveChatsAsync(Guid agentId)
        {
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == agentId);
            if (agent == null)
                return false;

            if (agent.ActiveChats < agent.MaxConcurrentChats)
            {
                agent.ActiveChats++;
                if (agent.ActiveChats >= agent.MaxConcurrentChats)
                    agent.IsAvailable = false;

                try
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task DecrementActiveChatsAsync(Guid agentId)
        {
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.Id == agentId);
            if (agent == null) return;
            if (agent.ActiveChats > 0)
            {
                agent.ActiveChats--;
                if (agent.ActiveChats < agent.MaxConcurrentChats)
                    agent.IsAvailable = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Agent agent)
        {
            _context.Agents.Update(agent);
            await _context.SaveChangesAsync();
        }
    }
}
