using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculaBlazor.Shared.DTOs;
using PeliculaBlazor.Shared.Entidades;

namespace PeliculaBlazor.Server.Controllers
{
    [ApiController]
    [Route("api/votos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class VotosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public VotosController(ApplicationDbContext context,UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Votar(VotoPeliculaDTO votoPeliculaDTO)
        {
            var usuario = await userManager.FindByEmailAsync(HttpContext.User.Identity!.Name!);
            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }
            var UsuarioId = usuario.Id;

            var votoActual = await context.VotoPeliculas
                .FirstOrDefaultAsync(x => x.PeliculaId == votoPeliculaDTO.PeliculaId
                && x.UsuarioId == usuario.Id);

            if (votoActual == null)
            {
                var votoPelicula = mapper.Map<VotoPelicula>(votoPeliculaDTO);
                votoPelicula.UsuarioId = usuario.Id;
                votoPelicula.FechaVoto = DateTime.Now;
                context.Add(votoPelicula);
            }
            else
            {
                votoActual.FechaVoto = DateTime.Now;
                votoActual.Voto = votoPeliculaDTO.Voto;
            }

            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
