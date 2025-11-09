using ProductApi.Application.Interfaces;
using ProductApi.Application.Services;
using ProductApi.Domain.Interfaces;
using ProductApi.Infrastructure.Providers;
using ProductApi.Infrastructure.Repositories;
using ProductApi.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddSingleton<IWeatherForecastProvider, RandomWeatherForecastProvider>();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapWeatherForecastEndpoints();

app.Run();
