using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeliculaBlazor.Shared.Entidades;

namespace PeliculaBlazor.Server
{
    public class ApplicationDbContext : IdentityDbContext 
    { 
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GeneroPelicula>().HasKey(x => new { x.GeneroId , x.PeliculaId });
            modelBuilder.Entity<PeliculaActor>().HasKey(x => new { x.ActorId, x.PeliculaId });

        }


        public DbSet<Genero> Generos => Set<Genero>();
        public DbSet<Actor> Actores => Set<Actor>();
        public DbSet<Pelicula> Peliculas => Set<Pelicula>();
        public DbSet<VotoPelicula> VotoPeliculas => Set<VotoPelicula>();

        public DbSet<GeneroPelicula> GenerosPeliculas => Set<GeneroPelicula>();
        public DbSet<PeliculaActor> PeliculasActor => Set<PeliculaActor>();


    }
}
