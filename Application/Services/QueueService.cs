using Application.Interfaces;
using Application.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class QueueService : IQueueService
    {
        private readonly ConcurrentQueue<ChatRequestDto> queue = new();

        public void Enqueue(ChatRequestDto request)
        {
            queue.Enqueue(request);
        }

        public ChatRequestDto? TryDequeue()
        {
            queue.TryDequeue(out var result);
            return result;
        }

        public int GetLength() => queue.Count;
    }
}
