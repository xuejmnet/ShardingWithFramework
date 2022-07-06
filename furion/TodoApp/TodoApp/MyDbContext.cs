using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding.Abstractions;

namespace TodoApp;

public class MyDbContext : AppShardingDbContext<MyDbContext>,IShardingTableDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public IRouteTail RouteTail { get; set; }
}