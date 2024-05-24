using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using IntegrationTests.Helpers;
using Xunit;
using DotNet.Testcontainers.Networks;

namespace IntegrationTests;

[CollectionDefinition("LikeServiceTests")]
public class LikeServiceTestFixture : IDisposable, ICollectionFixture<LikeServiceTestFixture>
{
    public Auth0Helper Auth0Helper { get; private set; }
    private readonly IDictionary<string, IContainer> _containers = new Dictionary<string, IContainer>();
    private readonly INetwork _network;

    public LikeServiceTestFixture()
    {
        Auth0Helper = new Auth0Helper();
        _network = new NetworkBuilder().Build();
    }

    public async Task<IContainer> StartRabbitMQContainerAsync()
    {
        var container = new ContainerBuilder()
            .WithImage("rabbitmq:3-management")
            .WithNetwork(_network)
            .WithNetworkAliases("rabbitmq")
            .WithPortBinding(15672, true)
            .WithPortBinding(5672, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete"))
            .Build();

        await container.StartAsync().ConfigureAwait(true);
        _containers["rabbitmq"] = container;

        return container;
    }

    public async Task<IContainer> StartMongoContainerAsync()
    {
        var container = new ContainerBuilder()
            .WithImage("mongo:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("mongo")
            .WithPortBinding(27017, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(27017))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Waiting for connections"))
            .Build();

        await container.StartAsync().ConfigureAwait(true);
        _containers["mongo"] = container;

        return container;
    }

    public async Task<IContainer> StartLikeServiceContainerAsync(IContainer mongoContainer, IContainer rabbitmqContainer)
    {
        var likeServiceContainer = new ContainerBuilder()
            .WithImage("rubyfeller/likeservice:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("likeservice")
            .WithPortBinding(8081, true)
            .WithEnvironment("LikeStoreDatabaseSettings__ConnectionString",
            $"mongodb://mongo:27017")
            .WithEnvironment("RabbitMQHost", "rabbitmq")
            .WithEnvironment("RabbitMQPort", "5672")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8081))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on"))
            .DependsOn(mongoContainer)
            .DependsOn(rabbitmqContainer)
            .Build();

        await likeServiceContainer.StartAsync().ConfigureAwait(true);
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
            .WithNetwork(_network)
            .WithNetworkAliases("postgres")
            .WithPortBinding(5432, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("database system is ready to accept connections"))
            .Build();

        await container.StartAsync();
        _containers["postgres"] = container;

        return container;
    }

    public async Task<IContainer> StartPostServiceContainerAsync(IContainer postgresContainer, IContainer rabbitmqContainer)
    {
        var postServiceContainer = new ContainerBuilder()
            .WithImage("rubyfeller/postservice:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("postservice")
            .WithPortBinding(8080, true)
            .WithEnvironment("ConnectionStrings__PostsConn",
            $"Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=postgres;")
            .WithEnvironment("RabbitMQHost", "rabbitmq")
            .WithEnvironment("RabbitMQPort", "5672")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on"))
            .DependsOn(postgresContainer)
            .DependsOn(rabbitmqContainer)
            .Build();

        await postServiceContainer.StartAsync();
        _containers["postservice"] = postServiceContainer;

        return postServiceContainer;
    }

    private async Task StopContainersAsync()
    {
        foreach (var container in _containers.Values)
        {
            await container.StopAsync();
        }

        await _network.DeleteAsync();
    }

    public void Dispose()
    {
        StopContainersAsync().Wait();
        GC.SuppressFinalize(this);
    }
}
