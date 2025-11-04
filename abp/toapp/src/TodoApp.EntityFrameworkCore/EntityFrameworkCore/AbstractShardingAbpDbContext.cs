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
using Volo.Abp.Domain.Entities.Events;
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
        private bool _createExecutor = false;

        protected AbstractShardingAbpDbContext(DbContextOptions<TDbContext> options) : base(options)
        {
        }


        private IShardingDbContextExecutor _shardingDbContextExecutor;

        public IShardingDbContextExecutor GetShardingExecutor()
        {
            if (!_createExecutor)
            {
                _shardingDbContextExecutor = this.DoCreateShardingDbContextExecutor();
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
                        if (dbContext is IAbpEfCoreDbContext abpEfCoreDbContext &&
                            this.UnitOfWorkManager.Current != null)
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

                    if (dbGeneratedAttr != null &&
                        dbGeneratedAttr.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
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

        /// <summary>
        /// abp 5.x+ 如果存在并发字段那么需要添加这段代码
        /// </summary>
        protected override void HandlePropertiesBeforeSave()
        {
            if (GetShardingExecutor() == null)
            {
                base.HandlePropertiesBeforeSave();
            }
        }

        protected override EntityEventReport CreateEventReport()
        {
            if (GetShardingExecutor() == null)
            {
                return base.CreateEventReport();
            }

            return new EntityEventReport();
        }

        /// <summary>
        /// abp 4.x+ 如果存在并发字段那么需要添加这段代码
        /// </summary>
        /// <returns></returns>

        // protected override void ApplyAbpConcepts(EntityEntry entry, EntityChangeReport changeReport)
        // {
        //     if (GetShardingExecutor() == null)
        //     {
        //         base.ApplyAbpConcepts(entry, changeReport);
        //     }
        // }
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