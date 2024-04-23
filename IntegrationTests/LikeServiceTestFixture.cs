using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace IntegrationTests;

[CollectionDefinition("LikeServiceTests")]
public class LikeServiceTestFixture : IDisposable, ICollectionFixture<LikeServiceTestFixture>
{
    private readonly IDictionary<string, IContainer> _containers = new Dictionary<string, IContainer>();

    public async Task<IContainer> StartRabbitMQContainerAsync()
    {
        var container = new ContainerBuilder()
            .WithImage("rabbitmq:3-management")
            .WithPortBinding(15672, true)
            .WithPortBinding(5672, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
            .Build();

        await container.StartAsync().ConfigureAwait(false);
        _containers["rabbitmq"] = container;
        return container;
    }

    public async Task<IContainer> StartMongoContainerAsync()
    {
        var container = new ContainerBuilder()
            .WithImage("mongo:latest")
            .WithPortBinding(27017, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(27017))
            .Build();

        await container.StartAsync().ConfigureAwait(false);
        _containers["mongo"] = container;
        return container;
    }

    public async Task<IContainer> StartLikeServiceContainerAsync(IContainer mongoContainer,
        IContainer rabbitmqContainer)
    {
        var mongoPort = mongoContainer.GetMappedPublicPort(27017);
        var rabbitmqPort = rabbitmqContainer.GetMappedPublicPort(5672);

        var likeServiceContainer = new ContainerBuilder()
            .WithImage("rubyfeller/likeservice:latest")
            .WithPortBinding(8081, true)
            .WithEnvironment("LikeStoreDatabaseSettings__ConnectionString",
                $"mongodb://host.docker.internal:{mongoPort}")
            .WithEnvironment("RabbitMQHost", "host.docker.internal")
            .WithEnvironment("RabbitMQPort", rabbitmqPort.ToString())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8081))
            .DependsOn(mongoContainer)
            .DependsOn(rabbitmqContainer)
            .Build();

        await likeServiceContainer.StartAsync().ConfigureAwait(false);
        _containers["likeservice"] = likeServiceContainer;
        return likeServiceContainer;
    }

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

        await container.StartAsync().ConfigureAwait(false);
        _containers["postgres"] = container;
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

        await postServiceContainer.StartAsync().ConfigureAwait(false);
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
        StopContainersAsync().GetAwaiter().GetResult();
    }
}