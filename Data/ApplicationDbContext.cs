using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Models;
using System;

namespace AgenciaDeViajes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Region> Regiones { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Festividad> Festividades { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<EntradaBlog> EntradasBlog { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<ReservaTour> ReservasTours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para manejo de fechas UTC
            modelBuilder.Entity<Contacto>()
                .Property(c => c.FechaContacto)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            // Relaciones existentes
            modelBuilder.Entity<Destino>()
                .HasOne(d => d.Region)
                .WithMany(r => r.Destinos)
                .HasForeignKey(d => d.id_region);

            // Agrega aquí más relaciones si es necesario
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Configuración global para DateTime (UTC)
            configurationBuilder.Properties<DateTime>()
                .HaveConversion(typeof(UtcValueConverter));
        }
    }

    public class UtcValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter() : base(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}