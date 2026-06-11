using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.Entities;

namespace TorneoFutbol.Infrastructure.Data
{
    public class TorneoFutbolDbContext : DbContext
    {
        public TorneoFutbolDbContext(DbContextOptions<TorneoFutbolDbContext> options)
        : base(options) { }

        // Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Torneo> Torneos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Gol> Goles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(150);
                e.HasIndex(u => u.Email).IsUnique(); // No puede haber dos usuarios con el mismo email
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.Rol).IsRequired().HasMaxLength(20).HasDefaultValue("Usuario");
            });

            // Torneo
            modelBuilder.Entity<Torneo>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Nombre).IsRequired().HasMaxLength(150);
                e.Property(t => t.Descripcion).HasMaxLength(500);
                e.Property(t => t.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Planificado");

                // Un torneo pertenece a un organizador (Usuario)
                // Si borrás el usuario, los torneos quedan sin organizador (restrict)
                e.HasOne(t => t.Organizador)
                 .WithMany(u => u.Torneos)
                 .HasForeignKey(t => t.OrganizadorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Equipo ──────────────────────────────────────────
            modelBuilder.Entity<Equipo>(e =>
            {
                e.HasKey(eq => eq.Id);
                e.Property(eq => eq.Nombre).IsRequired().HasMaxLength(100);
                e.Property(eq => eq.EscudoUrl).HasMaxLength(300);

                // Un equipo pertenece a un torneo
                // Si borrás el torneo, se borran sus equipos en cascada
                e.HasOne(eq => eq.Torneo)
                 .WithMany(t => t.Equipos)
                 .HasForeignKey(eq => eq.TorneoId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Jugador ─────────────────────────────────────────
            modelBuilder.Entity<Jugador>(e =>
            {
                e.HasKey(j => j.Id);
                e.Property(j => j.Nombre).IsRequired().HasMaxLength(100);
                e.Property(j => j.Posicion).IsRequired().HasMaxLength(10);
                e.Property(j => j.FotoUrl).HasMaxLength(300);
                e.Property(j => j.Activo).HasDefaultValue(true);

                // Un jugador pertenece a un equipo
                e.HasOne(j => j.Equipo)
                 .WithMany(eq => eq.Jugadores)
                 .HasForeignKey(j => j.EquipoId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Partido ─────────────────────────────────────────
            modelBuilder.Entity<Partido>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Pendiente");
                e.Property(p => p.Cancha).HasMaxLength(100);

                // Un partido pertenece a un torneo
                e.HasOne(p => p.Torneo)
                 .WithMany(t => t.Partidos)
                 .HasForeignKey(p => p.TorneoId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Relación con equipo local — NoAction para evitar múltiples cascade paths
                // SQL Server no permite más de un cascade path hacia la misma tabla
                e.HasOne(p => p.EquipoLocal)
                 .WithMany()
                 .HasForeignKey(p => p.EquipoLocalId)
                 .OnDelete(DeleteBehavior.NoAction);

                // Relación con equipo visitante — mismo motivo
                e.HasOne(p => p.EquipoVisitante)
                 .WithMany()
                 .HasForeignKey(p => p.EquipoVisitanteId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // ── Gol ─────────────────────────────────────────────
            modelBuilder.Entity<Gol>(e =>
            {
                e.HasKey(g => g.Id);
                e.Property(g => g.Minuto).IsRequired();
                e.Property(g => g.EsAutogol).HasDefaultValue(false);

                // Un gol pertenece a un partido
                e.HasOne(g => g.Partido)
                 .WithMany(p => p.Goles)
                 .HasForeignKey(g => g.PartidoId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Un gol lo hizo un jugador — NoAction para evitar cascade conflict
                e.HasOne(g => g.Jugador)
                 .WithMany(j => j.Goles)
                 .HasForeignKey(g => g.JugadorId)
                 .OnDelete(DeleteBehavior.NoAction);

                // Un gol pertenece a un equipo
                e.HasOne(g => g.Equipo)
                 .WithMany()
                 .HasForeignKey(g => g.EquipoId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
