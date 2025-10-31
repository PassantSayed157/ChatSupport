using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatSessionDto> CreateAndEnqueue(ChatRequestDto request);
        Task PollAsync(Guid sessionId);
        Task<IEnumerable<ChatSessionDto>> GetAllSessionsAsync();
        Task<ChatSessionDto?> GetByIdAsync(Guid sessionId);
    }
}
