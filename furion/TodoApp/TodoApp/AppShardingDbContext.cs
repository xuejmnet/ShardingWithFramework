using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using ShardingCore.EFCores;
using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding.ShardingDbContextExecutors;

namespace TodoApp;

public abstract class AppShardingDbContext<TDbContext> : AppShardingDbContext<TDbContext, MasterDbContextLocator>
    where TDbContext : DbContext
{
    protected AppShardingDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }
}

public abstract class AppShardingDbContext<TDbContext, TDbContextLocator>
    : AppDbContext<TDbContext, TDbContextLocator>, IShardingDbContext
    where TDbContext : DbContext
    where TDbContextLocator : class, IDbContextLocator
{
    private readonly IShardingDbContextExecutor? _shardingDbContextExecutor;

    protected AppShardingDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
        var wrapOptionsExtension = options.FindExtension<ShardingWrapOptionsExtension>();
        if (wrapOptionsExtension != null)
        {
            _shardingDbContextExecutor = new ShardingDbContextExecutor(this);
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
        return _shardingDbContextExecutor!;
    }
}