using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using AgenciaDeViajes.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity; // <-- Añadir este using para Identity
using Microsoft.AspNetCore.Identity.UI.Services; // <-- Opcional: para servicios de email

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

// Configuración de la cadena de conexión (usa Render o local)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ===== CONFIGURACIÓN DE IDENTITY CON CONFIRMACIÓN DE CORREO =====
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = true; // Requiere confirmación de email
    // Opcional: puedes agregar más configuraciones de Identity aquí
    // options.Password.RequireDigit = true;
    // etc...
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Autenticación: Cookies + Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login/Index";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
});

builder.Services.AddAuthorization();

// === REGISTRO DEL SERVICIO DE CLIMA ===
builder.Services.AddHttpClient<WeatherService>();

builder.Services.AddSingleton<TourPopularityService>();

// === OPCIONAL: Servicio para enviar emails (deberás implementarlo) ===
// builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Ejecutar migraciones automáticamente en producción
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // Esto crea/modifica tablas en Render
    
    // Opcional: puedes agregar aquí la creación de roles iniciales
    // o un usuario admin por defecto si lo necesitas
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

// === HABILITAR RUTAS DE IDENTITY (PARA LOGIN/REGISTER/ETC) ===
app.MapRazorPages(); // <-- Necesario para las páginas de Identity

app.Run();