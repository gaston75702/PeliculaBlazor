﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PeliculaBlazor.Server.Helpers;
using PeliculaBlazor.Shared.DTOs;
using PeliculaBlazor.Shared.Entidades;

namespace PeliculaBlazor.Server.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]

    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context,
            IAlmacenadorArchivos almacenadorArchivos,IMapper mapper , UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limite = 6;

            var peliculaEnCartelera = await context.Peliculas
                .Where(pelicula => pelicula.EnCartelera).Take(limite).OrderByDescending(pelicula => pelicula.Lanzamiento)
                .ToListAsync();

            var fechaActual = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(pelicula => pelicula.Lanzamiento > fechaActual)
                .OrderBy(pelicula => pelicula.Lanzamiento)
                .Take(limite).ToListAsync();

            var resultado = new HomePageDTO
            {
                PeliculasEnCartelera = peliculaEnCartelera,
                ProximosEstrenos = proximosEstrenos,
            };

            return resultado;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.Where(pelicula => pelicula.Id == id)
                .Include(pelicula => pelicula.GenerosPelicula)
                .ThenInclude(gp => gp.Genero)
                .Include(pelicula => pelicula.PeliculasActor.OrderBy(pa =>pa.Orden))
                .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync();

            if (pelicula is null)
            {
                return NotFound();
            }
            //sistema:Votacion
            var promedioVoto = 0.0;
            var votoUsuario = 0;

            if (await context.VotoPeliculas.AnyAsync(x => x.PeliculaId == id))
            {
                promedioVoto = await context.VotoPeliculas.Where(x => x.PeliculaId == id)
                    .AverageAsync(x => x.Voto);

                if (HttpContext.User.Identity!.IsAuthenticated)
                {
                    var usuario = await userManager.FindByEmailAsync(HttpContext.User.Identity!.Name!);
                    if (usuario is null)
                    {
                        return BadRequest("Usuario no encontrado");
                    }
                    var UsuarioId = usuario.Id;

                    var votoUsuarioDB = await context.VotoPeliculas
                        .FirstOrDefaultAsync(x => x.PeliculaId == id && x.UsuarioId == UsuarioId);

                    if(votoUsuarioDB is not null)
                    {
                        votoUsuario = votoUsuarioDB.Voto;
                    }
                }
            }


            var modelo = new PeliculaVisualizarDTO();
            modelo.Pelicula = pelicula;
            modelo.Generos = pelicula.GenerosPelicula.Select(gp => gp.Genero).ToList()!;
            modelo.Actores = pelicula.PeliculasActor.Select(pa => new Actor 
            {
                Nombre = pa.Actor!.Nombre,
                Foto = pa.Actor.Foto,
                Personaje= pa.Actor.Personaje,
                Id=pa.Actor.Id,
            }).ToList();

            modelo.PromedioVotos = promedioVoto;
            modelo.VotoUsuario = votoUsuario;
            return modelo;

        }

        [HttpGet("filtrar")]
        [AllowAnonymous]

        public async Task<ActionResult<List<Pelicula>>> Get([FromQuery] ParametrosBusquedaPeliculasDTO modelo)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();
            
            if(!string.IsNullOrWhiteSpace(modelo.Titulo))
            {
                peliculasQueryable =peliculasQueryable.Where(x=> x.Titulo.Contains(modelo.Titulo));
            }
            
            if(modelo.EnCartelera)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCartelera);
            }

            if (modelo.Estrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.Lanzamiento >= hoy);

            }
            if(modelo.GeneroId !=0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.GenerosPelicula
                    .Select(y => y.GeneroId)
                    .Contains(modelo.GeneroId));
            }


            if (modelo.MasVotadas)
            {
                peliculasQueryable = peliculasQueryable.OrderByDescending(p =>
                p.VotosPeliculas.Average(vp => vp.Voto));
            }

            await HttpContext.InsertarParametrosPaginacionEnRespuesta(peliculasQueryable, 
                modelo.CantidadRegistros);

            var peliculas = await peliculasQueryable.Paginar(modelo.PaginacionDTO).ToListAsync();
            return peliculas;

        }
        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculasActualizacionDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult) { return NotFound(); }

            var peliculaVisualizarDTO = peliculaActionResult.Value;
            var generosSeleccionadosIds = peliculaVisualizarDTO!.Generos.Select(x=> x.Id).ToList();
            var generosNoSeleccionados = await context.Generos
                .Where(x => !generosSeleccionadosIds.Contains(x.Id)).ToListAsync();

            var modelo = new PeliculasActualizacionDTO();
            modelo.Pelicula = peliculaVisualizarDTO.Pelicula;
            modelo.GenerosNoSeleccionados = generosNoSeleccionados;
            modelo.GenerosSeleccionados = peliculaVisualizarDTO.Generos;
            modelo.Actores = peliculaVisualizarDTO.Actores;
            return modelo;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var poster = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(poster, "jpg", contenedor);

            }

            EscribirOrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        private static void EscribirOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActor is not null)
            {
                for (int i = 0; i < pelicula.PeliculasActor.Count; i++)
                {
                    pelicula.PeliculasActor[i].Orden = i + 1;
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            var peliculaDB = await context.Peliculas.Include(x => x.GenerosPelicula)
                .Include(x => x.PeliculasActor)
                .FirstOrDefaultAsync(x=> x.Id == pelicula.Id);

            if(peliculaDB is null)
            {
                return NotFound();
            }
            peliculaDB = mapper.Map(pelicula,peliculaDB);

            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var posterImagen = Convert.FromBase64String(pelicula.Poster);
                peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(posterImagen
                    , ".jpg", contenedor, peliculaDB.Poster!);
            }
            
            EscribirOrdenActores(peliculaDB);

            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula is null) { return NotFound(); }
            context.Remove(pelicula);
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(pelicula.Poster!, contenedor);

            return NoContent();
        }
    }
}
