using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using AgenciaDeViajes.Services; // WeatherService
using AgenciaDeViajes.ML;        // SentimientoPredictionService
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace AgenciaDeViajes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Index";
                });

            builder.Services.AddAuthorization();

            // Weather service
            builder.Services.AddHttpClient<WeatherService>();

            builder.Services.AddSession();

            // Registro servicio ML y modelo
            builder.Services.AddSingleton<SentimientoPredictionService>(serviceProvider =>
            {
                var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var modeloPath = Path.Combine(env.ContentRootPath, "ML", "modelo_tours.zip");
                return new SentimientoPredictionService(modeloPath);
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

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

            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Rutas
            app.MapControllerRoute(
                name: "destinosLista",
                pattern: "ListaTours/Destination",
                defaults: new { controller = "ListaTours", action = "Destination" }
            );

            app.MapControllerRoute(
                name: "detallesDestinoSeo",
                pattern: "ListaTours/{nombreSeo}",
                defaults: new { controller = "ListaTours", action = "Details" }
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
