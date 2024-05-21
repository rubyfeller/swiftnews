using System.Text;
using System.Text.Json;
using PostService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PostService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageBusSubscriber(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.TryParse(_configuration["RabbitMQPort"], out int port) ? port : 5672
            };

            Console.WriteLine("Connection string: " + _configuration["RabbitMQHost"] + ":" + _configuration["RabbitMQPort"]);

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (ModuleHandle, ea) =>
                {
                    var body = ea.Body;
                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                    var messageType = GetMessageType(notificationMessage);
                    var message = DeserializeMessage(notificationMessage, messageType!);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

                        switch (message)
                        {
                            case Message postMessage when postMessage.action == "add":
                                Console.WriteLine("Adding like");
                                await eventProcessor.AddLike(postMessage.postId.ToString());
                                break;
                            case Message postMessage when postMessage.action == "remove":
                                Console.WriteLine("Removing like");
                                await eventProcessor.RemoveLike(postMessage.postId.ToString());
                                break;
                            case DeleteUserMessage deleteUserMessage:
                                Console.WriteLine("Removing user posts");
                                Console.WriteLine(deleteUserMessage.userId);
                                await eventProcessor.RemoveUserPosts(deleteUserMessage.userId);
                                break;
                        }
                    }
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private static Type? GetMessageType(string message)
        {
            if (message.Contains("removeUser"))
            {
                return typeof(DeleteUserMessage);
            }
            else if (message.Contains("action"))
            {
                return typeof(Message);
            }
            return null;
        }

        private static IMessage DeserializeMessage(string message, Type messageType)
        {
            if (messageType == null)
            {
                throw new InvalidOperationException("Unknown message type");
            }
            var deserializedMessage = JsonSerializer.Deserialize(message, messageType);
            return (IMessage)(deserializedMessage ?? throw new InvalidOperationException("Failed to deserialize message"));
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
