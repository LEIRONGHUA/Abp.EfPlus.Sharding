using Volo.Abp.Modularity;
using Xunit.Abstractions;

namespace Abp.EfPlus.Sharding.TestBase;

public class Customer_Tests<TStartupModule> : MyTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected Customer_Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
}