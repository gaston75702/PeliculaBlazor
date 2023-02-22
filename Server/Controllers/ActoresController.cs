using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculaBlazor.Server.Helpers;
using PeliculaBlazor.Shared.Entidades;


namespace PeliculaBlazor.Server.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenarArchivos;
        private readonly IMapper mapper;
        private readonly string contenedor = "personas";

        public ActoresController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos,IMapper mapper)
        {
            this.context = context;
            this.almacenarArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> Get()
        {
            return await context.Actores.ToListAsync();
        }

        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Actor>>> Get(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return new List<Actor>();
            }
            textoBusqueda= textoBusqueda.ToLower();

            return await context.Actores
                .Where(actor => actor.Nombre.ToLower().Contains(textoBusqueda))
                .Take(5)
                .ToListAsync();

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Actor>> Get(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(actor => actor.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            return actor;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Actor actor)
        {
            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor=Convert.FromBase64String(actor.Foto);
                actor.Foto = await almacenarArchivos.GuardarArchivo(fotoActor, "jpg", contenedor);

            }

            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }
        [HttpPut]
        public async Task<ActionResult> Put(Actor actor)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(a => a.Id == actor.Id);

            if(actorDB is null)
            {
                return NotFound();
            }
            actorDB = mapper.Map(actor,actorDB);

            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actorDB.Foto = await almacenarArchivos.EditarArchivo(fotoActor,".jpg",contenedor,actorDB.Foto!); 
            }

            await context.SaveChangesAsync();
            return NoContent();

        }
    }

}
