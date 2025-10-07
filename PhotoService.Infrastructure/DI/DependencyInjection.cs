using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotoService.Application.Interfaces;
using PhotoService.Application.Services;
using PhotoService.Domain.Interfaces;
using PhotoService.Infrastructure.Configuration;
using PhotoService.Infrastructure.Data;
using PhotoService.Infrastructure.Repositories;
using PhotoService.Infrastructure.Services;

namespace PhotoService.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            string connectionString,
            IConfigurationSection configSec,
            ILoggingBuilder logging)
        {
            // Configure EF Core DbContext
            services.AddDbContext<PhotoDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register repositories.
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IPhotoLikeRepository, PhotoLikeRepository>();
            services.AddScoped<IPhotoCategoryRepository, PhotoCategoryRepository>();
            services.AddScoped<IPhotoService, PhotoServiceImplementation>();
            services.AddScoped<IStorageService, GCSService>();
            services.AddScoped<IPhotoStorageService, PhotoStorageService>();

            // RabbitMQ service
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });


            // For RabbitMQSettings class, register it with the DI container.
            services.Configure<RabbitMQSettings>(configSec);

            services.AddSingleton<IRabbitMqService, RabbitMqService>();

            //services.AddSingleton<IRabbitMqService>(sp =>
            //{
            //    var logger = sp.GetRequiredService<ILogger<RabbitMqService>>();
            //    var service = new RabbitMqService(logger);
            //    service.InitializeAsync().GetAwaiter().GetResult();
            //    return service;
            //});

            return services;
        }
    }
}
