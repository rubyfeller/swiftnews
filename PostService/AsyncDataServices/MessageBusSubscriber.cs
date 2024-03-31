using System.Text;
using System.Text.Json;
using PostService.EventProcessing;
using PostService.Models;
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

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (ModuleHandle, ea) =>
                {
                    var body = ea.Body;
                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                    var message = JsonSerializer.Deserialize<Message>(notificationMessage);

                    if (message?.action != null)
                    {
                        Console.WriteLine(message.action + " message received");
                    }

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

                        if (message?.action == "add")
                        {
                            Console.WriteLine("Adding like");
                            await eventProcessor.AddLike(message.postId.ToString());
                        }
                        else if (message?.action == "remove")
                        {
                            Console.WriteLine("Removing like");
                            await eventProcessor.RemoveLike(message.postId.ToString());
                        }
                    }
                };


                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
