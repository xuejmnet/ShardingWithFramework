using Furion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ShardingCore;
using ShardingCore.Core.DbContextCreator;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Core.ShardingConfigurations.ConfigBuilders;
using ShardingCore.TableExists;
using ShardingCore.TableExists.Abstractions;
using TodoApp.Routes;

namespace TodoApp;

public class ShardingCoreComponent:IServiceComponent
{
    private static ILoggerFactory efLogger = LoggerFactory.Create(builder =>
    {
        builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
    });
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddDatabaseAccessor(options =>
        {
            // 配置默认数据库
            options.AddDb<MyDbContext>((sp,o) =>
            {
                ShardingCoreExtension.UseDefaultSharding<MyDbContext>(sp,o);
            });

        });
        //依赖注入
        services.AddShardingConfigure<MyDbContext>().UseRouteConfig(op =>
            {
                op.AddShardingTableRoute<TodoItemTableRoute>();
                op.AddShardingDataSourceRoute<TodoItemDataSourceRoute>();
            })
            .UseConfig((sp,op) =>
            {
                op.UseShardingQuery((con, b) =>
                {
                    b.UseMySql(con, new MySqlServerVersion(new Version()))
                        .UseLoggerFactory(efLogger);
                });
                op.UseShardingTransaction((con, b) =>
                {
                    b.UseMySql(con, new MySqlServerVersion(new Version()))
                        .UseLoggerFactory(efLogger);
                });
                op.AddDefaultDataSource("ds0", "server=127.0.0.1;port=3306;database=furion0;userid=root;password=root;");
                op.AddExtraDataSource(sp=>new Dictionary<string, string>()
                {
                    {"ds1", "server=127.0.0.1;port=3306;database=furion1;userid=root;password=root;"},
                    {"ds2", "server=127.0.0.1;port=3306;database=furion2;userid=root;password=root;"}
                });
                op.UseShardingMigrationConfigure(b =>
                {
                    b.ReplaceService<IMigrationsSqlGenerator, ShardingMySqlMigrationsSqlGenerator>();
                });
            }).ReplaceService<ITableEnsureManager,SqlServerTableEnsureManager>().AddShardingCore();
            
    }
}