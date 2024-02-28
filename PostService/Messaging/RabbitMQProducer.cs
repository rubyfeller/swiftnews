using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace PostService.Messaging
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public void SendTestMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal",
                Port = 5672
            };
            using (var connection = factory.CreateConnection())
            {
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "test",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish(exchange: "",
                    routingKey: "test",
                    basicProperties: null,
                    body: body);
            }
        }
    }
}