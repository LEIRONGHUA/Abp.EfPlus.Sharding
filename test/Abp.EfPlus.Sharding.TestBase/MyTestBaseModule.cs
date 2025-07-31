using Abp.EfPlus.Sharding.EfCore;
using Abp.EfPlus.Sharding.EfCore.Data;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Abp.EfPlus.Sharding.TestBase;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpGuidsModule),
    typeof(MyEfCoreModule)
)]
public class MyTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysAllowAuthorization();
        // var testOutputHelper = context.Services.GetRequiredService<ITestOutputHelper>();
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("../../../Logs/logs.log"))
            .WriteTo.Async(c => c.Console())
            // .WriteTo.TestOutput(testOutputHelper)
            .CreateLogger();
        context.Services.AddLogging(c => c.AddSerilog());
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using var scope = context.ServiceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<MyEfCoreRuntimeDatabaseMigrator>()
                .CheckAndApplyDatabaseMigrationsAsync();
        });
    }
}