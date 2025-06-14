using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using AgenciaDeViajes.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configuración existente de servicios
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configuración de base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configuración de servicios adicionales
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddSingleton<TourPopularityService>();

// Configuración de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Agencia de Viajes API",
        Description = "API para gestión de tours, destinos y reservas",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "soporte@agenciadeviajes.com",
            Url = new Uri("https://github.com/JeanEstrada-2004/AgenciaDeViajes")
        }
    });

    // Habilita anotaciones de Swagger
    c.EnableAnnotations();

    // Configuración del archivo XML de documentación
    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al cargar documentación XML: {ex.Message}");
    }
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agencia de Viajes API v1");
        c.RoutePrefix = "api-docs";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Middleware de autenticación (si lo necesitas)
// app.UseAuthentication();
// app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "destinosLista",
    pattern: "ListaTours/Destination",
    defaults: new { controller = "ListaTours", action = "Destination" });

app.MapControllerRoute(
    name: "detallesDestinoSeo",
    pattern: "ListaTours/{nombreSeo}",
    defaults: new { controller = "ListaTours", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();