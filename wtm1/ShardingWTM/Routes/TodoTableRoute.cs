using ShardingCore.Core.EntityMetadatas;
using ShardingCore.VirtualRoutes.Mods;
using ShardingWTM.Models;

namespace ShardingWTM.Routes;

public class TodoTableRoute:AbstractSimpleShardingModKeyStringVirtualTableRoute<Todo>
{
    public TodoTableRoute() : base(2, 3)
    {
    }

    /// <summary>
    /// 正常情况下不会用内容来做分片键因为作为分片键有个前提就是不会被修改
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(EntityMetadataTableBuilder<Todo> builder)
    {
        builder.ShardingProperty(o => o.Name);
    }
}