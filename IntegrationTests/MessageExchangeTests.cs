using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace IntegrationTests;

[Collection("MessageQueueTests")]
public class MessageExchangeTests
{
    private readonly MessageQueueTestFixture _fixture;

    public MessageExchangeTests(MessageQueueTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestMessageExchange()
    {
        // Arrange
        var rabbitmqContainer = await _fixture.StartRabbitMQContainerAsync();

        await Task.Delay(TimeSpan.FromSeconds(10));

        var factory = new ConnectionFactory()
        { HostName = "localhost", Port = rabbitmqContainer.GetMappedPublicPort(5672) };

        // Act
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "test_queue", durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        var tcs = new TaskCompletionSource<bool>();
        consumer.Received += (model, ea) =>
        {
            var receivedMessage = Encoding.UTF8.GetString(ea.Body.ToArray());

            // Assert
            Assert.Equal("post123", receivedMessage);
            tcs.SetResult(true);
        };
        channel.BasicConsume(queue: "test_queue", autoAck: true, consumer: consumer);

        var message = "post123";
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: "test_queue", basicProperties: null, body: body);

        await tcs.Task;
    }

    [Fact]
    public async Task TestLikeServiceSendsMessage()
    {
        // Arrange
        var rabbitmqContainer = await _fixture.StartRabbitMQContainerAsync();
        await Task.Delay(TimeSpan.FromSeconds(20));
        var mongoContainer = await _fixture.StartMongoContainerAsync();
        var likeServiceContainer = await _fixture.StartLikeServiceContainerAsync(mongoContainer, rabbitmqContainer);

        var factory = new ConnectionFactory() { HostName = "localhost", Port = rabbitmqContainer.GetMappedPublicPort(5672) };
        var httpClient = new HttpClient();

        // Act
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        var tcs = new TaskCompletionSource<string>();

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            tcs.SetResult(message);
        };
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        var likeServicePort = likeServiceContainer.GetMappedPublicPort(8081);
        var likeResponse = await httpClient.PostAsync($"http://localhost:{likeServicePort}/api/l/likes/7", null);
        likeResponse.EnsureSuccessStatusCode();

        if (await Task.WhenAny(tcs.Task, Task.Delay(10000)) == tcs.Task)
        {
            var receivedMessage = await tcs.Task;

            // Assert
            Assert.Equal("{\"action\":\"add\",\"postId\":7}", receivedMessage);
        }
        else
        {
            throw new TimeoutException("The test timed out waiting for the message to be received.");
        }
    }
}
