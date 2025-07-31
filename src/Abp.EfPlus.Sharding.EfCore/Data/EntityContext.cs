using Abp.EfPlus.Sharding.EfCore.Models;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Sharding.Abstractions;
using Volo.Abp.Data;

namespace Abp.EfPlus.Sharding.EfCore.Data;

[ConnectionStringName("Default")]
public class EntityContext : AbstractShardingAbpDbContext<EntityContext>, IShardingTableDbContext
{
    public EntityContext(DbContextOptions<EntityContext> options) : base(options)
    {
        
    }
    
    public DbSet<Customer> Customers { get; set; }
}