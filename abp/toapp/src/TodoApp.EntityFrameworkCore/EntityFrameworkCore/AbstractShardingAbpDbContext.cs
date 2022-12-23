using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShardingCore.Core.VirtualDatabase.VirtualDataSources;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.EFCores;
using ShardingCore.Extensions;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding.ShardingDbContextExecutors;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Reflection;

namespace TodoApp.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    /// Author: xjm
    /// Created: 2022/7/6 13:54:01
    /// Email: 326308290@qq.com
    public abstract class AbstractShardingAbpDbContext<TDbContext> : AbpDbContext<TDbContext>, IShardingDbContext
                                where TDbContext : DbContext
    {
        private readonly IShardingDbContextExecutor _shardingDbContextExecutor;
        protected AbstractShardingAbpDbContext(DbContextOptions<TDbContext> options) : base(options)
        {

            var wrapOptionsExtension = options.FindExtension<ShardingWrapOptionsExtension>();
            if (wrapOptionsExtension != null)
            {
                _shardingDbContextExecutor = new ShardingDbContextExecutor(this);
                _shardingDbContextExecutor.EntityCreateDbContextBefore += (sender, args) =>
                {
                    CheckAndSetShardingKeyThatSupportAutoCreate(args.Entity);
                };
                _shardingDbContextExecutor.CreateDbContextAfter += (sender, args) =>
                {
                    var dbContext = args.DbContext;
                    if (dbContext is AbpDbContext<TDbContext> abpDbContext && abpDbContext.LazyServiceProvider == null)
                    {
                        abpDbContext.LazyServiceProvider = this.LazyServiceProvider;
                        if (dbContext is IAbpEfCoreDbContext abpEfCoreDbContext)
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
        }

        public IShardingDbContextExecutor GetShardingExecutor()
        {
            return _shardingDbContextExecutor;
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
                AuditPropertySetter?.SetCreationProperties(entity);
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
}