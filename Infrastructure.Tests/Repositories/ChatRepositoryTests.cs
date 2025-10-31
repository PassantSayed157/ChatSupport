using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Mongo.Data;
using Infrastructure.Mongo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Repositories
{
    public class ChatRepositoryTests
    {
        private FakeMongoDbContext GetFakeContext()
        {
            return new FakeMongoDbContext();
        }

        private ChatRepository GetRepository(FakeMongoDbContext fake)
        {
            return new ChatRepository(fake);
        }

        [Fact]
        public async Task AddAsyncShouldInsertSession()
        {
            var fakeContext = GetFakeContext();
            var repo = GetRepository(fakeContext);

            var session = new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = "User1",
                CreatedAt = DateTime.UtcNow,
                Status = ChatStatus.Queued,
                Messages = new List<Message> { new Message { Text = "test", IsUser = true } }
            };

            await repo.AddAsync(session);

            var all = await fakeContext.FindAllAsync();
            all.Should().ContainSingle(s => s.UserId == "User1");
        }

        [Fact]
        public async Task UpdateAsyncShouldReplaceSession()
        {
            var fakeContext = GetFakeContext();
            var repo = GetRepository(fakeContext);

            var session = new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = "User2",
                Status = ChatStatus.Queued
            };

            await fakeContext.InsertOneAsync(session);

            session.Status = ChatStatus.Active;
            await repo.UpdateAsync(session);

            var updated = await fakeContext.FindByIdAsync(session.Id.ToString());
            updated.Status.Should().Be(ChatStatus.Active);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectSession()
        {
            var fakeContext = GetFakeContext();
            var repo = GetRepository(fakeContext);

            var session = new ChatSession { Id = Guid.NewGuid(), UserId = "User3" };
            await fakeContext.InsertOneAsync(session);

            var result = await repo.GetByIdAsync(session.Id);

            result.Should().NotBeNull();
            result.UserId.Should().Be("User3");
        }

        [Fact]
        public async Task GetAllAsyncShouldReturnAllSessions()
        {
            var fakeContext = GetFakeContext();
            var repo = GetRepository(fakeContext);

            await fakeContext.InsertOneAsync(new ChatSession { Id = Guid.NewGuid(), UserId = "User1" });
            await fakeContext.InsertOneAsync(new ChatSession { Id = Guid.NewGuid(), UserId = "User2" });

            var result = await repo.GetAllAsync();

            result.Should().HaveCount(2);
        }
    }
}
