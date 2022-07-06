using ShardingCore.Core.EntityMetadatas;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Core.VirtualRoutes.DataSourceRoutes.Abstractions;
using ShardingCore.Helpers;

namespace TodoApp.Routes;

public class TodoItemDataSourceRoute:AbstractShardingOperatorVirtualDataSourceRoute<TodoItem,string>
{
    /// <summary>
    /// id的hashcode取模余3分库
    /// </summary>
    /// <param name="shardingKey"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override string ShardingKeyToDataSourceName(object shardingKey)
    {
        if (shardingKey == null) throw new InvalidOperationException("sharding key cant null");
        var stringHashCode = ShardingCoreHelper.GetStringHashCode(shardingKey.ToString());
        return $"ds{(Math.Abs(stringHashCode) % 3)}";//ds0,ds1,ds2
    }

    private readonly List<string> _dataSources = new List<string>() { "ds0", "ds1", "ds2" };

    public override List<string> GetAllDataSourceNames()
    {
        return _dataSources;
    }

    public override bool AddDataSourceName(string dataSourceName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// id分库
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(EntityMetadataDataSourceBuilder<TodoItem> builder)
    {
        builder.ShardingProperty(o => o.Id);
    }

    public override Func<string, bool> GetRouteToFilter(string shardingKey, ShardingOperatorEnum shardingOperator)
    {
        var t = ShardingKeyToDataSourceName(shardingKey);
        switch (shardingOperator)
        {
            case ShardingOperatorEnum.Equal: return tail => tail == t;
            default:
            {
                return tail => true;
            }
        }
    }
}