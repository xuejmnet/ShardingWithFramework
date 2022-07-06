using Microsoft.EntityFrameworkCore;
using ShardingCore;

namespace TodoApp;

public class UseShardingCoreComponent:IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        app.ApplicationServices.UseAutoShardingCreate();
        var serviceProvider = app.ApplicationServices;
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var defaultShardingDbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            if (defaultShardingDbContext.Database.GetPendingMigrations().Any())
            {
                defaultShardingDbContext.Database.Migrate();
            }
        }
        // app.Services.UseAutoTryCompensateTable();
    }
}