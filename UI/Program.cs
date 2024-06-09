
using Lib.AspNetCore.ServerSentEvents;
using Entities.DataProvider;
using Services.Interface;
using Services.Services;
using StackExchange.Redis;

namespace SeatMapService
{
    public class Program
    {
        public static void Main(string[] args)
        {




            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
  
            builder.Services.AddCors(options => options.AddDefaultPolicy(o => o.AllowAnyOrigin()));
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConn");
                options.InstanceName = "GamesCatalog_";
            });

            
            builder.Services.AddSingleton<IDatabase>(cfg =>
            {
                IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn"));

          

                return multiplexer.GetDatabase();
            });

            builder.Services.AddInMemoryServerSentEventsNoReconnectClientsIdsStore();

            builder.Services.AddScoped<RedisCacheService>();
            builder.Services.AddServerSentEvents();
            builder.Services.AddServerSentEvents<INotificationsServerSentEventsService, NotificationsServerSentEventsService>();
            builder.Services.AddServerSentEvents<ISeatMapServerSentEventsService, SeatMapServerSentEventsService>();
            builder.Services.AddServerSentEventsClientIdProvider<ServerSentEventsClientIdProvider>();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

          
       
        

         
            app.MapServerSentEvents("/default-sse-endpoint");
            app.MapServerSentEvents<NotificationsServerSentEventsService>("/notifications-sse-endpoint");
            app.MapServerSentEvents<SeatMapServerSentEventsService>("/seatmap-sse-endpoint"); 
            app.MapControllers();
            app.Run();
        }
    }
}
