using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChatRepository
    {
        Task AddAsync(ChatSession session);
        Task UpdateAsync(ChatSession session);
        Task<ChatSession?> GetByIdAsync(Guid id);
        Task<List<ChatSession>> GetAllAsync();
    }
}
