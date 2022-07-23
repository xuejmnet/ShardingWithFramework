using EFCoreMigrateMultiDatabase;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=vs

var provider = builder.Configuration.GetValue("Provider", "UnKnown");

//Add-Migration InitialCreate -Context BlogContext -OutputDir Migrations\SqlServer -Args "--provider SqlServer"
//Add-Migration InitialCreate -Context BlogContext -OutputDir Migrations\MySql -Args "--provider MySql"
builder.Services.AddDbContext<MyDbContext>(options =>
{
    _ = provider switch
    {
        "MySql" => options.UseMySql("", new MySqlServerVersion(new Version())),
        "SqlServer" => options.UseSqlServer(""),
        _ => throw new Exception($"Unsupported provider: {provider}")
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();