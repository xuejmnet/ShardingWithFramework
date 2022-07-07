using Microsoft.EntityFrameworkCore;
using ShardingCore;
using ShardingCore.Core.DbContextCreator;
using ShardingCore.Core.ServiceProviders;
using ShardingCore.Sharding.Abstractions;
using WalkingTec.Mvvm.Core;

namespace ShardingWTM;

public class WTMDbContextCreator:IDbContextCreator
{
    public DbContext CreateDbContext(DbContext shellDbContext, ShardingDbContextOptions shardingDbContextOptions)
    {
        var context = new DataContext((DbContextOptions<DataContext>)shardingDbContextOptions.DbContextOptions);
        context.RouteTail = shardingDbContextOptions.RouteTail;
        return context;
    }

    public DbContext GetShellDbContext(IShardingProvider shardingProvider)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
        dbContextOptionsBuilder.UseDefaultSharding<DataContext>(ShardingCoreProvider.ShardingRuntimeContext);
        return new DataContext(dbContextOptionsBuilder.Options);
    }
}