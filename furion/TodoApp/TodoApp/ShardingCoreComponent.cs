using Furion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ShardingCore;
using ShardingCore.Core.DbContextCreator;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Core.ShardingConfigurations.ConfigBuilders;
using TodoApp.Routes;

namespace TodoApp;

public class ShardingCoreComponent:IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddDatabaseAccessor(options =>
        {
            // 配置默认数据库
            options.AddDb<MyDbContext>(o =>
            {
                o.UseDefaultSharding<MyDbContext>(ShardingCoreProvider.ShardingRuntimeContext);
            });

        });
        //依赖注入
        services.AddSingleton<IShardingRuntimeContext>(sp => ShardingCoreProvider.ShardingRuntimeContext);
    }
}