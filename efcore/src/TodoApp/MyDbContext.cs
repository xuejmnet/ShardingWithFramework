using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;
using TodoApp.Entities;

namespace TodoApp;

public class MyDbContext:AbstractShardingDbContext,IShardingTableDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public IRouteTail RouteTail { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TodoItem>(mb =>
        {
            mb.HasKey(o => o.Id);
            mb.Property(o => o.Id).IsRequired().HasMaxLength(50).HasComment("id");
            mb.Property(o => o.Text).IsRequired().HasMaxLength(256).HasComment("事情");
            mb.Property(o => o.Name).HasMaxLength(256).HasComment("姓名");
            mb.ToTable(nameof(TodoItem));
        });
        modelBuilder.Entity<TodoTest>(mb =>
        {
            mb.HasKey(o => o.Id);
            mb.Property(o => o.Id).IsRequired().HasMaxLength(50).HasComment("id");
            mb.Property(o => o.Test).IsRequired().HasMaxLength(256).HasComment("测试");
            mb.ToTable(nameof(TodoTest));
        });
    }
}