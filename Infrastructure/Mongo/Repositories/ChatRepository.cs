using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Mongo.Data;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly FakeMongoDbContext? _fake;
        private readonly MongoDbContext? _context;
        private readonly bool _useFake;

        public ChatRepository(FakeMongoDbContext fake)
        {
            _fake = fake ?? throw new ArgumentNullException(nameof(fake));
            _useFake = true;
        }

        public ChatRepository(MongoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _useFake = false;
        }

        public async Task AddAsync(ChatSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            if (_useFake)
            {
                await _fake!.InsertOneAsync(session);
                return;
            }

            await _context!.ChatSessions.InsertOneAsync(session);
        }

        public async Task UpdateAsync(ChatSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            if (_useFake)
            {
                await _fake!.ReplaceOneAsync(s => s.Id == session.Id, session);
                return;
            }

            await _context.ChatSessions.ReplaceOneAsync(s => s.Id == session.Id, session);
        }

        public async Task<ChatSession?> GetByIdAsync(Guid id)
        {
            if (_useFake)
            {
                return await _fake!.FindByIdAsync(id.ToString());
            }

            return await _context!.ChatSessions.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ChatSession>> GetAllAsync()
        {
            if (_useFake)
            {
                return await _fake!.FindAllAsync();
            }

            return await _context!.ChatSessions.Find(_ => true).ToListAsync();
        }
    }
}
