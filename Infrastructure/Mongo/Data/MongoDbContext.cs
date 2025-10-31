using Domain.Entities;
using Infrastructure.Mongo.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<ChatSession> ChatSessions => _db.GetCollection<ChatSession>("ChatSessions");
    }
}
