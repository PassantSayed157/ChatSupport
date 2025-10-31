using Application.Interfaces;
using Application.Services;
using Infrastructure;
using Infrastructure.EF;
using Infrastructure.EF.Repositories;
using Infrastructure.HostedServices;
using Infrastructure.Mongo.Config;
using Infrastructure.Mongo.Data;
using Infrastructure.Mongo.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Support Chat Service",
                    Version = "v1",
                    Description = "API for managing chat sessions and support agents"
                });
            });

            services.AddDbContext<EFContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            var useFake = _configuration.GetValue<bool>("UseInMemoryMongo");
            services.Configure<MongoSettings>(_configuration.GetSection("MongoSettings"));

            if (useFake)
            {
                services.AddSingleton<FakeMongoDbContext>();
                services.AddScoped<IChatRepository>(sp =>
                {
                    var fake = sp.GetRequiredService<FakeMongoDbContext>();
                    return new ChatRepository(fake);
                });
            }
            else
            {
                services.AddSingleton<MongoDbContext>();
                services.AddScoped<IChatRepository>(sp =>
                {
                    var ctx = sp.GetRequiredService<MongoDbContext>();
                    return new ChatRepository(ctx);
                });
            }

            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IAgentAssignmentService, AgentAssignmentService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IAgentRepository, AgentRepository>();

            services.Configure<SupportChatOptions>(_configuration.GetSection("SupportChatOptions"));
            services.AddHostedService(provider =>
            {
                var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
                var options = provider.GetRequiredService<IOptions<SupportChatOptions>>();
                return new QueueMonitorService(scopeFactory, options);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Support Chat Service V1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Console.WriteLine("Swagger ready at: https://localhost:5001/swagger");
        }
    }
}
