using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding.ReadWriteConfigurations;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.EfPlus.Sharding.EfCore.Data;

public static class ConfigureServicesExtensions
{
    public static void ConfigureSharding<T>(this IServiceCollection services, IConfiguration configuration)
        where T : AbpDbContext<T>, IShardingDbContext
    {
        // 直接固定为SqlServer数据库连接字符串
        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<T>(c => { c.DbContextOptions.UseDefaultSharding<T>(c.ServiceProvider); });
        });
        var connectionStringNameAttribute = typeof(T).GetCustomAttribute<ConnectionStringNameAttribute>();
        if (connectionStringNameAttribute == null)
        {
            throw new AbpException(
                $"The {nameof(ConnectionStringNameAttribute)} is not defined on the {typeof(T).FullName} class.");
        }

        // 分表组件单独配置内容
        services.AddShardingConfigure<T>()
            .UseRouteConfig(p => { })
            .UseConfig((sp, op) =>
            {
                op.UseShardingQuery((conStr, builder) =>
                {
                    builder.UseSqlServer(conStr).EnableSensitiveDataLogging().EnableDetailedErrors();
                });
                op.UseShardingTransaction((connection, builder) =>
                {
                    builder.UseSqlServer(connection).EnableSensitiveDataLogging().EnableDetailedErrors();
                });
                op.UseShardingMigrationConfigure(builder =>
                {
                    builder.ReplaceService<IMigrationsSqlGenerator, SqlServerShardingMigrationsSqlGenerator>();
                    builder.UseSqlServer(b =>
                        {
                            b.MigrationsHistoryTable($"__{connectionStringNameAttribute.Name}_Migrations");
                        })
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                });


                // 添加默认数据源
                op.AddDefaultDataSource("ds0",
                    configuration.GetConnectionString(connectionStringNameAttribute.Name));

                var customClassReadOnly =
                    configuration.GetSection($"ExtraConnections:{connectionStringNameAttribute.Name}ReadOnly");
                // ReSharper disable once InvertIf
                if (customClassReadOnly.Exists())
                {
                    // 读写分离配置 https://xuejmnet.github.io/sharding-core-doc/read-write/configure/#defaultenable
                    var readWriteSeparationConfigure =
                        customClassReadOnly.Get<Dictionary<string, IEnumerable<string>>>();
                    op.AddReadWriteSeparation(_ => readWriteSeparationConfigure,
                        readStrategyEnum: ReadStrategyEnum.Loop,
                        defaultEnableBehavior: ReadWriteDefaultEnableBehavior
                            .DefaultEnable,
                        defaultPriority: 10,
                        readConnStringGetStrategy: ReadConnStringGetStrategyEnum.LatestFirstTime);
                }
            }).AddShardingCore();
    }
}