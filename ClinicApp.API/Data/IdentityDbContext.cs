using Microsoft.EntityFrameworkCore;

namespace ClinicApp.API.Data
{
    public class IdentityDbContext<T>
    {
        private readonly DbContextOptions<ClinicContext> options;

        public IdentityDbContext(DbContextOptions<ClinicContext> options)
        {
            this.options = options;
        }
    }
}