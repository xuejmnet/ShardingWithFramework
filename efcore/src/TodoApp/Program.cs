using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ShardingCore;
using TodoApp;
using TodoApp.Routes;

 ILoggerFactory efLogger = LoggerFactory.Create(builder =>
{
    builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
});
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddShardingDbContext<MyDbContext>()
    .UseRouteConfig(op =>
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
        op.AddDefaultDataSource("ds0", "server=127.0.0.1;port=3306;database=mydb0;userid=root;password=root;");
        op.AddExtraDataSource(sp=>new Dictionary<string, string>()
        {
            {"ds1", "server=127.0.0.1;port=3306;database=mydb1;userid=root;password=root;"},
            {"ds2", "server=127.0.0.1;port=3306;database=mydb2;userid=root;password=root;"}
        });
        op.UseShardingMigrationConfigure(b =>
        {
            b.ReplaceService<IMigrationsSqlGenerator, ShardingMySqlMigrationsSqlGenerator>();
        });
    }).AddShardingCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 //如果有按时间分片的需要加定时任务否则可以不加
app.Services.UseAutoShardingCreate();
 
 using (var scope = app.Services.CreateScope())
 {
     var defaultShardingDbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
     if (defaultShardingDbContext.Database.GetPendingMigrations().Any())
     {
         defaultShardingDbContext.Database.Migrate();
     }
 }
 
 //如果需要在启动后扫描是否有表却扫了可以添加这个
 //app.Services.UseAutoTryCompensateTable();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();