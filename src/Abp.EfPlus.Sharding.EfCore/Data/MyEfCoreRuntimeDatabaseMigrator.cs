using Microsoft.Extensions.Logging;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Abp.EfPlus.Sharding.EfCore.Data;

public class MyEfCoreRuntimeDatabaseMigrator : EfCoreRuntimeDatabaseMigratorBase<EntityContext>
{
    private readonly MyDataSeeder _dataSeeder;

    public MyEfCoreRuntimeDatabaseMigrator(IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider, ICurrentTenant currentTenant, IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus, ILoggerFactory loggerFactory, MyDataSeeder dataSeeder)
        : base("Default", unitOfWorkManager, serviceProvider, currentTenant, abpDistributedLock, distributedEventBus,
            loggerFactory)
    {
        _dataSeeder = dataSeeder;
    }

    protected override async Task SeedAsync()
    {
        await _dataSeeder.SeedAsync();
    }
}