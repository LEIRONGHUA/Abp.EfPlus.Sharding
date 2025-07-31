using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
using Xunit.Abstractions;

namespace Abp.EfPlus.Sharding.TestBase;

/* All test classes are derived from this class, directly or indirectly. */
public abstract class MyTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ITestOutputHelper _testOutputHelper;
    protected MyTestBase(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("../../../Logs/logs.log"))
            .WriteTo.Async(c => c.Console())
            .WriteTo.TestOutput(_testOutputHelper)
            .CreateLogger();
    }
    
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    protected override void BeforeAddApplication(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", false);
        builder.AddJsonFile("appsettings.secrets.json", true);
        builder.AddJsonFile("appsettings.Development.json", true);
        services.ReplaceConfiguration(builder.Build());
    }

    protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                await action();

                await uow.CompleteAsync();
            }
        }
    }

    protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options,
        Func<Task<TResult>> func)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                var result = await func();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
}