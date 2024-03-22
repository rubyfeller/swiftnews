using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace LikeService.AsyncDataServices
{
    public class MessageBusClient: IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            var _configuration = configuration; 
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])};
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("Connected to Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to the MessageBus: {ex.Message}");
            }
        }

        public void AddLike() {
            var message = JsonSerializer.Serialize("1");

            if (_connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ connection open");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("RabbitMQ connection closed");
            }
        }


        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"Sent message: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus disposed");
            if (!_channel.IsOpen) return;
            _channel.Close();
            _connection.Close();
        }

        private static void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection has been shutdown");
        }
    }
}