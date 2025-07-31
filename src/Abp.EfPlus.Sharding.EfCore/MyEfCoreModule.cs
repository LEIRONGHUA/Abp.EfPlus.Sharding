using Abp.EfPlus.Sharding.EfCore.Data;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace Abp.EfPlus.Sharding.EfCore;

[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
public class MyEfCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddAbpDbContext<EntityContext>();
        context.Services.ConfigureSharding<EntityContext>(configuration);
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        using var scope = context.ServiceProvider.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<MyEfCoreRuntimeDatabaseMigrator>()
            .CheckAndApplyDatabaseMigrationsAsync();
    }
}