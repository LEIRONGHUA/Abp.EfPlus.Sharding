using Abp.EfPlus.Sharding.TestBase;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace Abp.EfPlus.Sharding.EfCore.Tests;

[DependsOn(
    typeof(MyTestBaseModule)
)]
public class MyEfCoreTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
    }
    
    
}