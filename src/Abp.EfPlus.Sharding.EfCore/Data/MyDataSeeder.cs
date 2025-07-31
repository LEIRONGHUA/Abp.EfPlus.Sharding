using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Abp.EfPlus.Sharding.EfCore.Data;

public class MyDataSeeder : ITransientDependency
{
    private readonly ILogger<MyDataSeeder> _logger;
    private readonly IDataSeeder _dataSeeder;

    public MyDataSeeder(ILogger<MyDataSeeder> logger, IDataSeeder dataSeeder)
    {
        _logger = logger;
        _dataSeeder = dataSeeder;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        await _dataSeeder.SeedAsync(new DataSeedContext(tenantId));
    }
}