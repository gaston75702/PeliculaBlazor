using PeliculaBlazor.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculaBlazor.Client.Repositorios
{
    public class Repositorio : IRepositorio
    {
        public List<Pelicula> ObtenerPeliculas()
        {
            return new List<Pelicula>()
            {

                new Pelicula(){Titulo="Spiderman 3",Lanzamiento=new DateTime(2008,7,2),Poster="https://cl.buscafs.com/www.tomatazos.com/public/uploads/images/147906/147906_600x890.jpg"},
                new Pelicula(){Titulo="Los tres chanchitos ",Lanzamiento=new DateTime(2017,5,4),Poster="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRQHuq6Fni2wkFEBM1-wwTuxdvPZc1cKyHddw&usqp=CAU"},
                new Pelicula(){Titulo="Django",Lanzamiento=new DateTime(2019,4,11),Poster="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVD_eFBddLSXWLMdXTEbFwDA7ZaZ7DT3YU8aV7MoCQic7gej-YKgPZktLB_fd9kE8e5xQ&usqp=CAU" }
            };
        }
    }
}
