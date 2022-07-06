using ShardingCore.Core.EntityMetadatas;
using ShardingCore.VirtualRoutes.Mods;
using TodoApp.Entities;

namespace TodoApp.Routes;

public class TodoItemTableRoute:AbstractSimpleShardingModKeyStringVirtualTableRoute<TodoItem>
{
    public TodoItemTableRoute() : base(2, 3)
    {
    }

    /// <summary>
    /// 正常情况下不会用内容来做分片键因为作为分片键有个前提就是不会被修改
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(EntityMetadataTableBuilder<TodoItem> builder)
    {
        builder.ShardingProperty(o => o.Text);
    }
}