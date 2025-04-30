using MalyFarmar.Api.DAL;
using MalyFarmar.Api.DAL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var sqliteDbDefaultName = builder.Configuration.GetConnectionString("DefaultConnection") ??
                          throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var dataFolder = Environment.SpecialFolder.LocalApplicationData;
var basePath = Environment.GetFolderPath(dataFolder);
var connectionString = $"Data Source={Path.Combine(basePath, sqliteDbDefaultName)}";

builder.Services.AddDbContext<MalyFarmarDbContext>(options =>
    options
        .UseSqlite(
            connectionString,
            x => x
                .UseNetTopologySuite()
                .MigrationsAssembly("MalyFarmar.Api.DAL")
            ));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "MalyFarmar API",
            Version = "v1"
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();