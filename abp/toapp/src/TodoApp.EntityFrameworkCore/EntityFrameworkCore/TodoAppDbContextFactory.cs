using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
using TodoApp.Routes;

namespace TodoApp.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
    * (like Add-Migration and Update-Database commands) */
    public class TodoAppDbContextFactory : IDesignTimeDbContextFactory<TodoAppDbContext>
    {
        private static IServiceProvider _serviceProvider;
        static TodoAppDbContextFactory()
        {
            var services = new ServiceCollection();

            services.AddShardingDbContext<TodoAppDbContext>()
                .UseRouteConfig(op =>
                {
                    op.AddShardingDataSourceRoute<TodoDataSourceRoute>();
                    op.AddShardingTableRoute<TodoTableRoute>();
                })
                .UseConfig((sp, op) =>
                {
                    op.UseShardingQuery((conStr, builder) =>
                    {
                        builder.UseSqlServer(conStr);
                    });
                    op.UseShardingTransaction((connection, builder) =>
                    {
                        builder.UseSqlServer(connection);
                    });
                    op.UseShardingMigrationConfigure(builder =>
                    {
                        builder.ReplaceService<IMigrationsSqlGenerator, ShardingSqlServerMigrationsSqlGenerator>();
                    });
                    op.AddDefaultDataSource("ds0", "Server=localhost,1434;Database=TodoApp;User Id=sa;password=myPassw0rd;TrustServerCertificate=True");
                    op.AddExtraDataSource(sp =>
                    {
                        return new Dictionary<string, string>()
                        {
                            { "ds1", "Server=localhost,1434;Database=TodoApp1;User Id=sa;password=myPassw0rd;TrustServerCertificate=True" },
                            { "ds2", "Server=localhost,1434;Database=TodoApp2;User Id=sa;password=myPassw0rd;TrustServerCertificate=True" }
                        };
                    });
                })
                .AddShardingCore();
            _serviceProvider = services.BuildServiceProvider();
        }
        public TodoAppDbContext CreateDbContext(string[] args)
        {
            TodoAppEfCoreEntityExtensionMappings.Configure();



            return _serviceProvider.GetService<TodoAppDbContext>();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TodoApp.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
