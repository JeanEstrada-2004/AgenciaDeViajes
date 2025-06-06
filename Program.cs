using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using AgenciaDeViajes.Services; // <-- Asegúrate de agregar esto

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

// Configuración de la cadena de conexión (usa Render o local)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Habilitar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Login/Index";
    });

builder.Services.AddAuthorization();

// === REGISTRO DEL SERVICIO DE CLIMA ===
builder.Services.AddHttpClient<WeatherService>(); // <-- Este es el registro PRO

// === HABILITAR SESSION ===
builder.Services.AddSession(); // <-- LÍNEA CLAVE

var app = builder.Build();

// Ejecutar migraciones automáticamente en producción
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // Esto crea/modifica tablas en Render
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// === HABILITAR SESSION EN EL PIPELINE ===
app.UseSession(); // <-- LÍNEA CLAVE

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// === RUTA EXPLÍCITA PARA DESTINATION ===
app.MapControllerRoute(
    name: "destinosLista",
    pattern: "ListaTours/Destination",
    defaults: new { controller = "ListaTours", action = "Destination" }
);

// === RUTA SEO PARA DETALLES DE DESTINO ===
app.MapControllerRoute(
    name: "detallesDestinoSeo",
    pattern: "ListaTours/{nombreSeo}",
    defaults: new { controller = "ListaTours", action = "Details" }
);

// === RUTA POR DEFECTO ===
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
