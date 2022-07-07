using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ShardingCore;
using ShardingCore.Core.DbContextCreator;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.TableExists;
using ShardingCore.TableExists.Abstractions;
using TodoApp.Routes;

namespace TodoApp;

public class ShardingCoreProvider
{
    private static ILoggerFactory efLogger = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });
    private static readonly IShardingRuntimeContext instance;
    public static IShardingRuntimeContext ShardingRuntimeContext => instance;
    static ShardingCoreProvider()
    {
        instance=new ShardingRuntimeBuilder<MyDbContext>().UseRouteConfig(op =>
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
            }).ReplaceService<ITableEnsureManager,SqlServerTableEnsureManager>(ServiceLifetime.Singleton).ReplaceService<IDbContextCreator, CustomerDbContextCreator>(ServiceLifetime.Singleton).Build();
    }
}