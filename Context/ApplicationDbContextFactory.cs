using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Organizador.Context
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
        private readonly string _path;

        public ApplicationDbContextFactory(string path)
        {
            _path = path;
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            string dataBasePath = Path.Combine(_path, "organizador.db");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dataBasePath}");
            return new(optionsBuilder.Options);
        }
    }
}

