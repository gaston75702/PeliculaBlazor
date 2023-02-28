using PeliculaBlazor.Shared.DTOs;

namespace PeliculaBlazor.Client.Auth
{
    public interface ILoginService
    {
        Task Login(UserTokenDTO tokenDTO);
        Task Logout();
        Task ManejarRenovacionToken();
    }
}
