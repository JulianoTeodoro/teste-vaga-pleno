
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Parking.Api.Business;
using Parking.Api.Data;
using Parking.Api.Data.Repositories;
using Parking.Api.Interfaces.Business;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection
var conn = builder.Configuration.GetConnectionString("Postgres")
           ?? "Host=localhost;Port=5432;Database=parking_test;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(conn);
});

builder.Services.AddScoped<PlacaService>();

// Business
builder.Services.AddScoped<IClienteBusiness, ClienteBusiness>();
builder.Services.AddScoped<IVeiculosBusiness, VeiculosBusiness>();
builder.Services.AddScoped<IFaturamentoBusiness, FaturamentoBusiness>();

// Repositories
builder.Services.AddScoped<IClienteEFRepository, ClienteEFRepository>();
builder.Services.AddScoped<IVeiculosEFRepository, VeiculosEFRepository>();
builder.Services.AddScoped<IFaturaEFRepository, FaturaEFRepository>();
builder.Services.AddScoped<IFaturaVeiculoEFRepository, FaturaVeiculoEFRepository>();
builder.Services.AddScoped<IClienteVeiculoVigenciaEFRepository, ClienteVeiculoVigenciaEFRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Parking API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // porta do Vite/React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
