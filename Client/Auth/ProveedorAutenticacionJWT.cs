using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using PeliculaBlazor.Client.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace PeliculaBlazor.Client.Auth
{
    public class ProveedorAutenticacionJWT : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;

        public ProveedorAutenticacionJWT(IJSRuntime js,HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        }

        public static readonly string TOKENKEY = "TOKENKEY";

        private AuthenticationState Anonimo =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await js.ObtenerDeLocalStorage(TOKENKEY);

            if (token is null)
            {
                return Anonimo;
            }

            return ConstruirAuthenticationState(token.ToString()!);

        }

        private AuthenticationState ConstruirAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("bearer", token);

            var claims = ParsearClaimsDelJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,"jwt")));
        }
        private IEnumerable<Claim> ParsearClaimsDelJWT(string token)
        {
            var jwtSecuriyTokenHandler = new JwtSecurityTokenHandler();
            var tokenDeserializado =jwtSecuriyTokenHandler.ReadJwtToken(token);
            return tokenDeserializado.Claims;
        }

        public async Task Login(string token)
        {
            await js.GuardarEnLocalStorage(TOKENKEY, token);
            var authState = ConstruirAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));


        }

        public async Task Logout()
        {
            await js.RemoverDeLocalStorage(TOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
    }
}
