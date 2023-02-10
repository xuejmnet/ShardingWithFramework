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
using WalkingTec.Mvvm.Core;
using DbContextOptions = Microsoft.EntityFrameworkCore.DbContextOptions;
using ShardingCore.Extensions;

namespace ShardingWTM
{

    public abstract class AbstractShardingFrameworkContext:FrameworkContext, IShardingDbContext
    {

        public AbstractShardingFrameworkContext(CS cs)
            : base(cs)
        {
            
        }
        
        public AbstractShardingFrameworkContext(string cs, DBTypeEnum dbtype)
            : base(cs, dbtype)
        {
        }
        
        public AbstractShardingFrameworkContext(string cs, DBTypeEnum dbtype, string version = null)
            : base(cs, dbtype, version)
        {
        }

        public AbstractShardingFrameworkContext(DbContextOptions options) : base(options)
        {

        }
        private bool _createExecutor = false;
        private IShardingDbContextExecutor _shardingDbContextExecutor;
        
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


        public IShardingDbContextExecutor GetShardingExecutor()
        {
            if (!_createExecutor)
            {
                _shardingDbContextExecutor=this.CreateShardingDbContextExecutor();
                _createExecutor = true;
            }
            return _shardingDbContextExecutor;
        }
    }
}