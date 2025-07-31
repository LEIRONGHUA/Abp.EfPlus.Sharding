using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Update;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Helpers;

namespace Abp.EfPlus.Sharding.EfCore.Data;

/// <summary>
/// 作用是在执行Migrator程序自动迁移时，可以创建分表结构。
/// </summary>
public class SqlServerShardingMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
{
    private readonly IShardingRuntimeContext _shardingRuntimeContext;

    public SqlServerShardingMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies,
        ICommandBatchPreparer commandBatchPreparer, IShardingRuntimeContext shardingRuntimeContext) : base(dependencies, commandBatchPreparer)
    {
        _shardingRuntimeContext = shardingRuntimeContext;
    }
    
    protected override void Generate(
        MigrationOperation operation,
        IModel? model,
        MigrationCommandListBuilder builder)
    {
        var oldCmds = builder.GetCommandList().ToList();
        base.Generate(operation, model, builder);
        var newCmds = builder.GetCommandList().ToList();
        var addCmds = newCmds.Where(x => !oldCmds.Contains(x)).ToList();

        MigrationHelper.Generate(_shardingRuntimeContext, operation, builder, Dependencies.SqlGenerationHelper,
            addCmds);
    }
}