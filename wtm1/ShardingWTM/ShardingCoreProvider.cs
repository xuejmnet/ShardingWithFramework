using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShardingCore;
using ShardingCore.Core.DbContextCreator;
using ShardingCore.Core.RuntimeContexts;
using ShardingWTM.Routes;

namespace ShardingWTM;

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
        instance=new ShardingRuntimeBuilder<DataContext>().UseRouteConfig(op =>
            {
                op.AddShardingTableRoute<TodoTableRoute>();
                op.AddShardingDataSourceRoute<TodoDataSourceRoute>();
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
                op.AddDefaultDataSource("ds0", "server=127.0.0.1;port=3306;database=wtm0;userid=root;password=root;");
                op.AddExtraDataSource(sp=>new Dictionary<string, string>()
                {
                    {"ds1", "server=127.0.0.1;port=3306;database=wtm1;userid=root;password=root;"},
                    {"ds2", "server=127.0.0.1;port=3306;database=wtm2;userid=root;password=root;"}
                });
                op.UseShardingMigrationConfigure(b =>
                {
                    b.ReplaceService<IMigrationsSqlGenerator, ShardingMySqlMigrationsSqlGenerator>();
                });
            }).ReplaceService<IDbContextCreator, WTMDbContextCreator>(ServiceLifetime.Singleton).Build();
    }
}