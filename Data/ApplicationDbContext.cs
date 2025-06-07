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



        public DbSet<Region> Regiones { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Festividad> Festividades { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Actividad> Actividades { get; set; }

        public DbSet<EntradaBlog> EntradasBlog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones existentes
            modelBuilder.Entity<Destino>()
                .HasOne(d => d.Region)
                .WithMany(r => r.Destinos)
                .HasForeignKey(d => d.id_region);

            // Agrega aquí más relaciones si es necesario
        }
        
        public DbSet<ReservaTour> ReservasTours { get; set; }
    }
}
