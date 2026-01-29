using LRU.LFU.Caching.Extensions;
using LRU.LFU.Caching.Implementations;
using LRU.LFU.Caching.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add caching services
builder.Services.AddCachingServices(builder.Configuration);
// Register typed caches
builder.Services.AddSingleton<ILruCache<int, string>>(_ =>
    new LruCache<int, string>(50));

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
