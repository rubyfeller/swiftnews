using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace PostService.Messaging;

public class RabbitMQProducer : IRabbitMQProducer
{
    private readonly IModel _channel;

    public RabbitMQProducer(IOptions<RabbitMQOptions> options)
    {
        var factory = new ConnectionFactory()
        {
            HostName = options.Value.HostName,
            ClientProvidedName = options.Value.ClientProvidedName,
            Port = options.Value.Port
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "pubsub",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void SendTestMessage<T>(T message)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(
                exchange: "",
                routingKey: "test",
                basicProperties: null,
                body: body);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}