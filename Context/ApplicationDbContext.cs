using Microsoft.EntityFrameworkCore;
using Organizador.Entities;

namespace Organizador.Context
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Folder> Folders => Set<Folder>();
		public DbSet<FileExtension> FileExtensions => Set<FileExtension>();
	}
}

