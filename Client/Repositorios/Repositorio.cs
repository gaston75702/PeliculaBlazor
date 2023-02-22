using PeliculaBlazor.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PeliculaBlazor.Client.Repositorios
{
    public class Repositorio : IRepositorio
    {
        private readonly HttpClient httpClient;

        public Repositorio(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private JsonSerializerOptions OpcionesPorDefectoJSON => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive= true
        };
        public async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            var respuestaHTTP = await httpClient.GetAsync(url);
            if (respuestaHTTP.IsSuccessStatusCode)
            {
                var respuesta = await DeserializarRespuesta<T>(respuestaHTTP, OpcionesPorDefectoJSON);
                return new HttpResponseWrapper<T>(respuesta, error: false, respuestaHTTP);
            }
            else
            {
                return new HttpResponseWrapper<T>(default,error: true, respuestaHTTP);
            }
        }
            
        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T enviar)
        {
            var enviarJSON = JsonSerializer.Serialize(enviar);
            var enviarContent= new StringContent(enviarJSON, Encoding.UTF8 ,"application/json");
            var responseHttp= await httpClient.PostAsync(url, enviarContent);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);

        }
        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T enviar)
        {
            var enviarJSON = JsonSerializer.Serialize(enviar);
            var enviarContent = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
            var responseHttp = await httpClient.PutAsync(url, enviarContent);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);

        }

        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar)
        {
            var enviarJSON = JsonSerializer.Serialize(enviar);
            var enviarContent = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
            var responseHttp = await httpClient.PostAsync(url, enviarContent);

            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await DeserializarRespuesta<TResponse>(responseHttp
                    , OpcionesPorDefectoJSON);

                return new HttpResponseWrapper<TResponse>(response, error: false, responseHttp);
            }

            return new HttpResponseWrapper<TResponse>(default, !responseHttp.IsSuccessStatusCode,responseHttp);

        }
        private async Task<T> DeserializarRespuesta<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var respuestaString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(respuestaString, jsonSerializerOptions);
        }
         
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
