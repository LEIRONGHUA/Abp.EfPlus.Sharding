using Abp.EfPlus.Sharding.EfCore.Data;
using Abp.EfPlus.Sharding.EfCore.Models;
using Abp.EfPlus.Sharding.TestBase;
using Xunit;
using Xunit.Abstractions;
using Z.EntityFramework.Plus;

namespace Abp.EfPlus.Sharding.EfCore.Tests;

public class EfCoreCustomer_Tests : Customer_Tests<MyEfCoreTestModule>
{
    private readonly EntityContext _entityContext;

    public EfCoreCustomer_Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _entityContext = GetRequiredService<EntityContext>();
    }

    [Fact]
    public async Task UpdateAsyncTest()
    {
        await _entityContext.Customers.Where(x => x.Name == "Customer_C")
            .UpdateAsync(x => new Customer { Description = "Updated_C", IsActive = false });
    }

    [Fact]
    public async Task BulkMergeAsyncTest()
    {
        var customers = new[]
        {
            new Customer
                { Name = "Customer_A", Description = "Description_A", LastLogin = DateTime.Now, IsActive = true },
            new Customer
            {
                Name = "Customer_B", Description = "Description_B", LastLogin = DateTime.Now.AddDays(-1),
                IsActive = true
            },
            new Customer
            {
                Name = "Customer_C", Description = "Description_C", LastLogin = DateTime.Now.AddDays(-2),
                IsActive = true
            }
        };
        // entityContext.BulkMergeAsync is ok, _entityContext.Customers.BulkMergeAsync is not ok
        await _entityContext.Customers.BulkMergeAsync(customers, options =>
        {
            options.ColumnPrimaryKeyExpression = x => x.Name; // 设置主键映射
        });
    }
}