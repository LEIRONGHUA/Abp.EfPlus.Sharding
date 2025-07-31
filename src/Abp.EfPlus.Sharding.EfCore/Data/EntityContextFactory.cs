using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;

namespace Abp.EfPlus.Sharding.EfCore.Data;

public class EntityContextFactory : IDesignTimeDbContextFactory<EntityContext>
{
    private static readonly IServiceProvider ServiceProvider;

    static EntityContextFactory()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = BuildConfiguration();
        serviceCollection.AddShardingDbContext<EntityContext>();
        serviceCollection.ConfigureSharding<EntityContext>(configuration);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }


    public EntityContext CreateDbContext(string[] args)
    {
        return ServiceProvider.GetRequiredService<EntityContext>();
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.Development.json", optional: true);
        return builder.Build();
    }
}