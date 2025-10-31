using Domain.Entities;
using Domain.Enums;
using Infrastructure.EF;
using Infrastructure.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Repositories
{
    public class AgentRepositoryTests
    {
        private EFContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<EFContext>()
                .UseInMemoryDatabase(databaseName: "Fakedb" + Guid.NewGuid())
                .Options;

            var context = new EFContext(options);

            var agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Name = "userA", Seniority = SeniorityLevel.Junior, IsAvailable = true, ActiveChats = 0 },
                new Agent { Id = Guid.NewGuid(), Name = "userB", Seniority = SeniorityLevel.Mid, IsAvailable = true, ActiveChats = 10 },
                new Agent { Id = Guid.NewGuid(), Name = "userC", Seniority = SeniorityLevel.Senior, IsAvailable = false, ActiveChats = 2 },
            };

            context.Agents.AddRange(agents);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetAvailableAgentsAsyncShouldReturnOnlyAvailableWithCapacity()
        {
            var context = GetInMemoryContext();
            var repo = new AgentRepository(context);

            var result = await repo.GetAvailableAgentsAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task TryIncrementActiveChatsAsyncShouldIncrementWhenAgentHasCapacity()
        {
            var context = GetInMemoryContext();
            var repo = new AgentRepository(context);
            var agent = context.Agents.First(a => a.Name == "userA");
            var oldCount = agent.ActiveChats;

            var success = await repo.TryIncrementActiveChatsAsync(agent.Id);

            Assert.True(success);
            Assert.Equal(oldCount + 1, agent.ActiveChats);
        }

        [Fact]
        public async Task TryIncrementActiveChatsAsyncShouldReturnFalseWhenAgentNotFound()
        {
            var context = GetInMemoryContext();
            var repo = new AgentRepository(context);

            var success = await repo.TryIncrementActiveChatsAsync(Guid.NewGuid());

            Assert.False(success);
        }

        [Fact]
        public async Task DecrementActiveChatsAsyncShouldDecreaseAndSetAvailableWhenUnderLimit()
        {
            var context = GetInMemoryContext();
            var repo = new AgentRepository(context);
            var agent = context.Agents.First(a => a.Name == "userB");
            agent.IsAvailable = false;

            await repo.DecrementActiveChatsAsync(agent.Id);

            Assert.True(agent.IsAvailable);
        }

        [Fact]
        public async Task UpdateAsyncShouldUpdateAgentSuccessfully()
        {
            var context = GetInMemoryContext();
            var repo = new AgentRepository(context);
            var agent = context.Agents.First(a => a.Name == "userA");

            agent.IsAvailable = false;
            await repo.UpdateAsync(agent);

            var updated = await context.Agents.FindAsync(agent.Id);
            Assert.False(updated.IsAvailable);
        }
    }
}
