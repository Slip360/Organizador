using Microsoft.EntityFrameworkCore;
using Organizador.Context;
using Organizador.Entities;

namespace Organizador.Configurations
{
	public class FileExtensionService : IDisposableAsync
	{
        private readonly ApplicationDbContext _applicationDbContext;
        public bool IsDisposed { get; set; } = false;

        public FileExtensionService(ApplicationDbContext applicationDbContext)
		{
            _applicationDbContext = applicationDbContext;
		}

        /// <summary>
        /// Crea una extensión de archivo.
        /// </summary>
        /// <param name="fileExtension">Datos a guardar.</param>
        /// <returns>Tarea que retorna void.</returns>
        public async Task CreateFileExtensionAsync(FileExtension fileExtension)
        {
            await _applicationDbContext.FileExtensions.AddAsync(fileExtension);
            await _applicationDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina la extensión que tiene el id.
        /// </summary>
        /// <param name="fileExtensionId">Id de la extensión.</param>
        /// <returns>Tarea que retorna void.</returns>
        public async Task DeleteFileExtensionAsync(int fileExtensionId)
        {
            var fileExtension = await _applicationDbContext.FileExtensions
                .Where((extension) => extension.Id == fileExtensionId)
                .FirstOrDefaultAsync() ?? throw new Exception("No se ha encontrado la extensión. Error al eliminar.");
            _applicationDbContext.FileExtensions.Remove(fileExtension);
            await _applicationDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene las extensiones junto con el folder.
        /// </summary>
        /// <returns>Tarea que retorna las extensiones.</returns>
        public async Task<IEnumerable<FileExtension>> GetFileExtensionsWithFolderAsync() =>
            await _applicationDbContext.FileExtensions
            .Include((file) => file.Folder)
            .ToListAsync();

        public async Task DisposeAsync()
        {
            await _applicationDbContext.DisposeAsync();
            IsDisposed = true;
        }
    }
}

