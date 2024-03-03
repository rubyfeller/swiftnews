using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

while (true)
{
    var factory = new ConnectionFactory
    {
        HostName = "host.docker.internal",
        ClientProvidedName = "test",
        Port = 5672
    };
    var connection = factory.CreateConnection();

    using
    var channel = connection.CreateModel();

    channel.QueueDeclare(queue: "test",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, eventArgs) =>
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Test message received: {message}");
    };
    channel.BasicConsume(queue: "test", autoAck: true, consumer: consumer);

    // Sleep for a while before the next iteration
    System.Threading.Thread.Sleep(1000);
}