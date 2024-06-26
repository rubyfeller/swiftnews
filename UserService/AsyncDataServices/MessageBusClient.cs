using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace UserService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"] };

            Console.WriteLine("Connection string: " + _configuration["RabbitMQHost"] + ":" + _configuration["RabbitMQPort"]);

            var portString = _configuration["RabbitMQPort"];
            if (portString != null && int.TryParse(portString, out int port))
            {
                factory.Port = port;
            }

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("Connected to Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to the MessageBus: {ex.Message}");
            }
        }

        public void RemoveUser(string userId)
        {
            SendMessage(new { action = "removeUser", userId });
        }

        private void SendMessage(object message)
        {
            if (_channel == null)
            {
                Console.WriteLine("Channel is not initialized");
                return;
            }

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "pubsub", routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"Sent message: {jsonMessage}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus disposed");
            if (_channel != null && !_channel.IsOpen) return;
            _channel?.Close();
            _connection?.Close();
        }

        private static void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection has been shutdown");
        }
    }
}
