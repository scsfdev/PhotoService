using Microsoft.Extensions.Logging;
using PhotoService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ValidateCategoryEvents;

namespace PhotoService.Infrastructure.Services
{
    public class RabbitMqService(ILogger<RabbitMqService> logger) : IRabbitMqService, IAsyncDisposable
    {
        private IConnection connection;
        private IChannel channel;

        private readonly string requestQueue = "validate_category_request";
        private readonly string responseQueue = "validate_category_response";
        private readonly Dictionary<Guid, TaskCompletionSource<bool>> pendingRequests = new();


        public async Task InitializeAsync()
        {
            // Initialize RabbitMQ connection here
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // Replace with your RabbitMQ server hostname
                Port = 5672,            // Replace with your RabbitMQ server port
                UserName = "guest",     // Replace with your RabbitMQ username
                Password = "guest"      // Replace with your RabbitMQ password
            };

            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            logger.LogInformation("RabbitMQ connection and channel initialized successfully.");

            // Declare queues.
            await channel.QueueDeclareAsync(queue: requestQueue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            await channel.QueueDeclareAsync(queue: responseQueue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Set up consumer for response queue
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var response = JsonSerializer.Deserialize<ValidateCategoryGuidResponseEvent>(body);
                    if (response != null && pendingRequests.TryGetValue(response.RequestId, out var tcs))
                    {
                        tcs.SetResult(response.IsValid);
                        pendingRequests.Remove(response.RequestId);

                        logger.LogInformation("Received response for RequestId {RequestId}, IsValid={IsValid}", response.RequestId, response.IsValid);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing RabbitMQ message.");
                    
                }
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue: responseQueue,
                                     autoAck: true,
                                     consumer: consumer);
        }

        public async Task<bool> ValidateCategoryAsync(Guid categoryGuid, CancellationToken cancellationToken = default)
        {
            var requestId = Guid.NewGuid();
            var tcs = new TaskCompletionSource<bool>();
            pendingRequests[requestId] = tcs;
            var validateEvent = new ValidateCategoryGuidEvent
            {
                RequestId = requestId,
                CategoryGuid = categoryGuid
            };
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(validateEvent));

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: requestQueue, body: messageBody);

            logger.LogInformation("Published ValidateCategoryGuidEvent with RequestId {RequestId}", requestId);

            return tcs.Task.Result;
        }

        public async ValueTask DisposeAsync()
        {
            if (channel != null && channel.IsOpen)
            {
                await channel.CloseAsync();
                await channel.DisposeAsync();
            }
            else if(channel != null)
            {
                await channel.DisposeAsync();
            }

            if (connection != null && connection.IsOpen)
            {
                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
            else if(connection != null)
            {
                await connection.DisposeAsync();
            }
        }
    }
}
