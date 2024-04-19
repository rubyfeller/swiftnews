using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace IntegrationTests;

[CollectionDefinition("PostServiceTests")]
public class PostServiceTestFixture : IDisposable, ICollectionFixture<PostServiceTestFixture>
{
    private readonly IDictionary<string, IContainer> _containers = new Dictionary<string, IContainer>();

    public async Task<IContainer> StartPostgresContainerAsync()
    {
        var container = new ContainerBuilder()
            .WithImage("postgres:latest")
            .WithEnvironment("POSTGRES_USER", "postgres")
            .WithEnvironment("POSTGRES_PASSWORD", "postgres")
            .WithEnvironment("POSTGRES_DB", "postgres")
            .WithPortBinding(5432, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("database system is ready to accept connections"))
            .Build();

        await container.StartAsync();
        _containers["postgres"] = container;
        return container;
    }

    public async Task<IContainer> StartRabbitMQContainerAsync()
    {
        var container = new ContainerBuilder()
            .WithImage("rabbitmq:3-management")
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
            .Build();

        await container.StartAsync();
        _containers["rabbitmq"] = container;
        return container;
    }

    public async Task<IContainer> StartPostServiceContainerAsync(IContainer postgresContainer,
        IContainer rabbitmqContainer)
    {
        var postgresPort = postgresContainer.GetMappedPublicPort(5432);
        var rabbitmqPort = rabbitmqContainer.GetMappedPublicPort(5672);

        var postServiceContainer = new ContainerBuilder()
            .WithImage("rubyfeller/postservice:latest")
            .WithPortBinding(8080, true)
            .WithEnvironment("ConnectionStrings__DefaultConnection",
                $"Server=host.docker.internal;Port={postgresPort};Database=postgres;User Id=postgres;Password=postgres;")
            .WithEnvironment("RabbitMQPort", rabbitmqPort.ToString())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
            .DependsOn(postgresContainer)
            .DependsOn(rabbitmqContainer)
            .Build();

        await postServiceContainer.StartAsync();
        return postServiceContainer;
    }

    private async Task StopContainersAsync()
    {
        foreach (var container in _containers.Values)
        {
            await container.StopAsync();
        }
    }

    public void Dispose()
    {
        StopContainersAsync().Wait();
        GC.SuppressFinalize(this);
    }
}