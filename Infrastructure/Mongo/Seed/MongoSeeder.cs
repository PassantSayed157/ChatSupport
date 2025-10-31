using Domain.Entities;
using Domain.Enums;
using Infrastructure.Mongo.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mongo.Seed
{
    public static class MongoSeeder
    {
        public static async Task SeedAsync(MongoDbContext ctx)
        {
            var count = await ctx.ChatSessions.CountDocumentsAsync(FilterDefinition<ChatSession>.Empty);
            if (count == 0)
            {
                var demoSession = new ChatSession
                {
                    UserId = "User1",
                    Status = ChatStatus.Completed,
                    Messages = new List<Message>
                    {
                        new Message { Text = "Hello!", IsUser = true },
                        new Message { Text = "Hi! How can I help you?", IsUser = false }
                    }
                };
                await ctx.ChatSessions.InsertOneAsync(demoSession);
            }
        }
    }
}
