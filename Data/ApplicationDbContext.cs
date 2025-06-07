using Microsoft.EntityFrameworkCore;
using AgenciaDeViajes.Models;

namespace AgenciaDeViajes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablas principales existentes
        public DbSet<Region> Regiones { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Festividad> Festividades { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Actividad> Actividades { get; set; }

        // --- Agrega los DbSet para el flujo de reservas ---
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<PasajeroReserva> PasajerosReserva { get; set; }
        public DbSet<ReservaServicioAdicional> ReservaServiciosAdicionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones para Destino <-> Region
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Destino>()
                .HasOne(d => d.Region)
                .WithMany(r => r.Destinos)
                .HasForeignKey(d => d.id_region);

            // Puedes agregar aquí futuras configuraciones de relaciones
        }
    }
}
