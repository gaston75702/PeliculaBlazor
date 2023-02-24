using AutoMapper;
using PeliculaBlazor.Shared.Entidades;

namespace PeliculaBlazor.Server.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Actor, Actor>()
                .ForMember(x => x.Foto, option => option.Ignore());
            
            CreateMap<Pelicula, Pelicula>()
                .ForMember(x => x.Poster, option => option.Ignore());

        }
    }
}
