namespace PeliculaBlazor.Client.Auth
{
    public interface ILoginService
    {
        Task Login(string token);
        Task Logout();
    }
}
