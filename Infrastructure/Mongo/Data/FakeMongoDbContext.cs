using Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mongo.Data
{
    public class FakeMongoDbContext
    {
        private readonly ConcurrentDictionary<string, ChatSession> _sessions = new();

        public Task InsertOneAsync(ChatSession session)
        {
            _sessions.TryAdd(session.Id.ToString(), session);
            return Task.CompletedTask;
        }

        public Task ReplaceOneAsync(Func<ChatSession, bool> predicate, ChatSession session)
        {
            _sessions[session.Id.ToString()] = session;
            return Task.CompletedTask;
        }

        public Task<ChatSession?> FindByIdAsync(string id)
        {
            _sessions.TryGetValue(id, out var session);
            return Task.FromResult(session);
        }

        public Task<List<ChatSession>> FindAllAsync()
        {
            var list = _sessions.Values.ToList();
            return Task.FromResult(list);
        }

        public Task ClearAsync()
        {
            _sessions.Clear();
            return Task.CompletedTask;
        }
    }

}
