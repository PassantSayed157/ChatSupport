using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.HostedServices
{
    public class QueueMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan interval;
        private readonly int maxMissedPolls;

        public QueueMonitorService(IServiceScopeFactory scopeFactory, IOptions<SupportChatOptions> options)
        {
            _scopeFactory = scopeFactory;
            interval = TimeSpan.FromSeconds(options.Value.PollIntervalSeconds);
            maxMissedPolls = options.Value.MaxMissedPolls;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var chatRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                    var agentRepo = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

                    var sessions = await chatRepo.GetAllAsync();

                    foreach (var s in sessions.Where(x => x.Status == ChatStatus.Active))
                    {
                        var idleSeconds = (DateTime.Now - s.LastPollTime).TotalSeconds;

                        if (idleSeconds > maxMissedPolls * interval.TotalSeconds)
                        {
                            s.Status = ChatStatus.Inactive;
                            await chatRepo.UpdateAsync(s);

                            if (s.AssignedAgentId is not null)
                                await agentRepo.DecrementActiveChatsAsync(s.AssignedAgentId.Value);
                        }
                    }
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
