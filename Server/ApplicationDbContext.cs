using Microsoft.EntityFrameworkCore;

namespace PeliculaBlazor.Server
{
    public class ApplicationDbContext : DbContext
    { 
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

    }
}
