using Microsoft.EntityFrameworkCore;
using ShardingCore;
using ShardingCore.Core.DbContextCreator;
using ShardingCore.Core.ServiceProviders;

namespace TodoApp;
public class CustomerDbContextCreator:ActivatorDbContextCreator<MyDbContext>
{
    public override DbContext GetShellDbContext(IShardingProvider shardingProvider)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        dbContextOptionsBuilder.UseDefaultSharding<MyDbContext>(ShardingCoreComponent.ShardingCoreContext);
        return new MyDbContext(dbContextOptionsBuilder.Options);
    }
}