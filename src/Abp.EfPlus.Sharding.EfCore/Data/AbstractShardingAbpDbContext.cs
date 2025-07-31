using System.ComponentModel.DataAnnotations.Schema;
using Abp.EfPlus.Sharding.EfCore.ShardingKeys;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Extensions;
using ShardingCore.Sharding.Abstractions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Reflection;

namespace Abp.EfPlus.Sharding.EfCore.Data;


/// <summary>
/// 继承sharding-core接口
/// 封装实现抽象类
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class AbstractShardingAbpDbContext<TDbContext> : AbpDbContext<TDbContext>, IShardingDbContext
                                where TDbContext : DbContext
{

    public IRouteTail RouteTail { get; set; } = null!;
    
    private bool _createExecutor = false;
    protected AbstractShardingAbpDbContext(DbContextOptions<TDbContext> options) : base(options)
    {

    }
    private IShardingDbContextExecutor? _shardingDbContextExecutor;
    public IShardingDbContextExecutor? GetShardingExecutor()
    {
        if (!_createExecutor)
        {
            _shardingDbContextExecutor = DoCreateShardingDbContextExecutor();
            _createExecutor = true;
        }
        return _shardingDbContextExecutor;
    }

    private IShardingDbContextExecutor DoCreateShardingDbContextExecutor()
    {
        var shardingDbContextExecutor = this.CreateShardingDbContextExecutor();
        if (shardingDbContextExecutor != null)
        {

            shardingDbContextExecutor.EntityCreateDbContextBefore += (sender, args) =>
            {
                CheckAndSetShardingKeyThatSupportAutoCreate(args.Entity);
            };
            shardingDbContextExecutor.CreateDbContextAfter += (sender, args) =>
            {
                var dbContext = args.DbContext;
                if (dbContext is AbpDbContext<TDbContext> abpDbContext && abpDbContext.LazyServiceProvider == null)
                {
                    abpDbContext.LazyServiceProvider = this.LazyServiceProvider;
                    if (dbContext is IAbpEfCoreDbContext abpEfCoreDbContext && this.UnitOfWorkManager.Current != null)
                    {
                        abpEfCoreDbContext.Initialize(
                            new AbpEfCoreDbContextInitializationContext(
                                this.UnitOfWorkManager.Current
                            )
                        );
                    }
                }
            };
        }
        return shardingDbContextExecutor;
    }

    private void CheckAndSetShardingKeyThatSupportAutoCreate<TEntity>(TEntity entity) where TEntity : class
    {
        if (entity is IShardingKeyIsGuId)
        {

            if (entity is IEntity<Guid> guidEntity)
            {
                if (guidEntity.Id != default)
                {
                    return;
                }
                var idProperty = entity.GetObjectProperty(nameof(IEntity<Guid>.Id));

                var dbGeneratedAttr = ReflectionHelper
                    .GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(
                        idProperty
                    );

                if (dbGeneratedAttr != null && dbGeneratedAttr.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
                {
                    return;
                }

                EntityHelper.TrySetId(
                    guidEntity,
                    () => GuidGenerator.Create(),
                    true
                );
            }
        }
        else if (entity is IShardingKeyIsCreationTime)
        {
            AuditPropertySetter.SetCreationProperties(entity);
        }
    }

    /// <summary>
    /// abp 5.x+ 如果存在并发字段那么需要添加这段代码
　　 /// 种子数据需要
    /// </summary>
    protected override void HandlePropertiesBeforeSave()
    {
        if (GetShardingExecutor() == null)
        {
            base.HandlePropertiesBeforeSave();
        }
    }

    public override void Dispose()
    {
        _shardingDbContextExecutor?.Dispose();
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_shardingDbContextExecutor != null)
        {
            await _shardingDbContextExecutor.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}