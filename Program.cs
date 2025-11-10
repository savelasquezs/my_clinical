using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Clinica_Herramientas_2.Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);

// Leer configuración
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("ClinicaDb") 
    ?? throw new InvalidOperationException("Connection string 'ClinicaDb' not found.");

// Registrar servicios de la aplicación
builder.Services.AddClinicaServices(connectionString);

// Agregar controladores
builder.Services.AddControllers();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Clínica Herramientas 2 API",
        Version = "v1",
        Description = "API REST para el sistema de gestión de clínica"
    });
});

var app = builder.Build();

// Configurar el pipeline HTTP
// Habilitar Swagger siempre (para proyecto de demostración)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clínica Herramientas 2 API v1");
    c.RoutePrefix = "swagger"; // Hacer que Swagger esté disponible en /swagger
});

// Comentar UseHttpsRedirection si no tienes certificado HTTPS configurado
// app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
