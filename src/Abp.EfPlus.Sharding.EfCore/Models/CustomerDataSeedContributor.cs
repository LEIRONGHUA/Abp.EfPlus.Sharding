using Abp.EfPlus.Sharding.EfCore.Data;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Abp.EfPlus.Sharding.EfCore.Models;

public class CustomerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly EntityContext _entityContext;

    public CustomerDataSeedContributor(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var initList = new List<Customer>()
        {
            new Customer
            {
                Name = "Customer_A", Description = "Description", IsActive = true, LastLogin = new DateTime(2010, 1, 1)
            },
            new Customer
            {
                Name = "Customer_B", Description = "Description", IsActive = true, LastLogin = new DateTime(2010, 1, 1)
            },
            new Customer { Name = "Customer_C", Description = "Description", IsActive = true, LastLogin = DateTime.Now }
        };
        var allList = await _entityContext.Customers.ToListAsync();

        foreach (var item in initList.Where(item => allList.All(p => p.Name != item.Name)))
        {
            _entityContext.Customers.Add(item);
        }

        await _entityContext.SaveChangesAsync();
    }
}