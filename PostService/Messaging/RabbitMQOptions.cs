namespace PostService.Messaging;

public class RabbitMQOptions
{
    public string HostName { get; set; }
    public string ClientProvidedName { get; set; }
    public int Port { get; set; }
}