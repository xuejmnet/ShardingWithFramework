using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShardingCore;
using ShardingCore.Core.VirtualDatabase.VirtualDataSources;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.EFCores;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding.ShardingDbContextExecutors;
using WalkingTec.Mvvm.Core;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using DbContextOptions = Microsoft.EntityFrameworkCore.DbContextOptions;

namespace ShardingWTM
{

    public abstract class AbstractShardingFrameworkContext:FrameworkContext, IShardingDbContext
    {
        protected IShardingDbContextExecutor ShardingDbContextExecutor
        {
            get;
        }

        public AbstractShardingFrameworkContext(CS cs)
            : base(cs)
        {
            
            ShardingDbContextExecutor =new ShardingDbContextExecutor(this);
        }
        
        public AbstractShardingFrameworkContext(string cs, DBTypeEnum dbtype)
            : base(cs, dbtype)
        {
            ShardingDbContextExecutor =new ShardingDbContextExecutor(this);
        }
        
        public AbstractShardingFrameworkContext(string cs, DBTypeEnum dbtype, string version = null)
            : base(cs, dbtype, version)
        {
            ShardingDbContextExecutor =new ShardingDbContextExecutor(this);
        }

        public AbstractShardingFrameworkContext(DbContextOptions options) : base(options)
        {
            var wrapOptionsExtension = options.FindExtension<ShardingWrapOptionsExtension>();
            if (wrapOptionsExtension != null)
            {
                ShardingDbContextExecutor =new ShardingDbContextExecutor(this);;
            }

        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (this.CSName!=null)
            {
                base.OnConfiguring(optionsBuilder);
                optionsBuilder.UseDefaultSharding<DataContext>(ShardingCoreProvider.ShardingRuntimeContext);
            }
        }
        public override void Dispose()
        {
            ShardingDbContextExecutor?.Dispose();
            base.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            if (ShardingDbContextExecutor != null)
            {
                await ShardingDbContextExecutor.DisposeAsync();
            }

            await base.DisposeAsync();
        }

        public IShardingDbContextExecutor GetShardingExecutor()
        {
            return ShardingDbContextExecutor;
        }
    }
}