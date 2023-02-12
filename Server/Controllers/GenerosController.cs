using PeliculaBlazor.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PeliculaBlazor.Server.Controllers
{
    [Route("api/generos")]
    [ApiController]

    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public GenerosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genero>>> Get()
        {
            return await context.Generos.ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post(Genero genero)
        {

            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }
    }    
}
