using Microsoft.EntityFrameworkCore;
using Organizador.Context;
using Organizador.Entities;

namespace Organizador.Configurations
{
	public class FolderService : IDisposableAsync
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public bool IsDisposed { get; set; } = false;

        public FolderService(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		/// <summary>
		/// Crea un folder.
		/// </summary>
		/// <param name="folder">Datos a guardar.</param>
		/// <returns>Tarea que retorna void.</returns>
		public async Task CreatedFolderAsync(Folder folder)
		{
			await _applicationDbContext.Folders.AddAsync(folder);
			await _applicationDbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Elimina el folder que tiene el id.
		/// </summary>
		/// <param name="folderId">Id del folder.</param>
		/// <returns>Tarea que retorna void.</returns>
		public async Task DeleteFolderAsync(int folderId)
		{
			var folder = await GetFoldersAsQuery()
				.Where((_folder) => _folder.Id == folderId)
				.Include((_folder) => _folder.FileExtensions)
				.FirstOrDefaultAsync() ?? throw new Exception("El folder no existe. Error al eliminar.");
			folder.FileExtensions.Clear();
			_applicationDbContext.Folders.Remove(folder);
			await _applicationDbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Obtiene las carpetas como una query.
		/// </summary>
		/// <returns>Query de las carpetas.</returns>
		public IQueryable<Folder> GetFoldersAsQuery() =>
			_applicationDbContext.Folders;

		/// <summary>
		/// Obtienes las carpetas y sus extensiones como una query.
		/// </summary>
		/// <returns>Query con las extensiones.</returns>
		public IQueryable<Folder> GetFoldersWithFileExtensionsAsQuery() =>
			_applicationDbContext.Folders
			.Include((folder) => folder.FileExtensions);

        public async Task DisposeAsync()
        {
			await _applicationDbContext.DisposeAsync();
			IsDisposed = true;
        }
    }
}

