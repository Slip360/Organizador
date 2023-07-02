using Microsoft.EntityFrameworkCore;
using Organizador.Entities;

namespace Organizador.Configurations
{
	public class MenuService
	{
		private readonly string[] _menuOptions = {
            "Agregar carpeta.",
            "Agregar extensiones a carpeta.",
            "Eliminar carpeta.",
            "Eliminar extensiones.",
            "Ver datos.",
            "Ordenar archivos de una carpeta",
			"Cerrar."
        };

		/// <summary>
		/// Imprime el menú de opciones del programa (antes de hacer la impresión limpia la terminal).
		/// </summary>
		public void PrintMenu()
		{
			Console.Clear();
			int totalItems = _menuOptions.Length;
			for(int i = 0; i < totalItems; i++)
			{
				int itemNumber = i + 1;
				Console.WriteLine(string.Format("{0}) {1}", itemNumber, _menuOptions[i]));
			}
            Console.Write("Seleccionar opción: ");
        }

        /// <summary>
        /// Construye un objeto de tipo Folder para ser guardado en la base de datos.
        /// </summary>
        /// <returns>Retorna el objeto si ingresa un nombre correcto en caso contrario null.</returns>
        /// <exception cref="Exception"></exception>
        public Folder CreateFolder()
		{
			Console.Clear();
			Console.Write("Nombre del folder: ");
			string? name = Console.ReadLine() ?? throw new Exception("No se ha ingresado un nombre válido.");

            return new()
            {
                FolderName = name
            };
		}

        /// <summary>
        /// Muestra un menú en pantalla y retorna el id del folder a eliminar.
        /// </summary>
        /// <param name="folders">Query que tiene las carpetas.</param>
        /// <returns>Retorna el id del folder.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> DeleteFolderAsync(IQueryable<Folder> folders)
        {
            Console.Clear();
            if (await folders.AnyAsync() == false)
                throw new Exception("No se han encontrado carpetas.");

            Console.WriteLine("Si elimina el folder todas las extensiones también serán eliminadas.");
            int folderNumber = 1;
            await folders.ForEachAsync((folder) =>
            {
                Console.WriteLine(string.Format("{0}) Id: {1}. Nombre: {2}", folderNumber, folder.Id, folder.FolderName));
            });

            Console.Write("Id del folder a eliminar: ");
            bool result = int.TryParse(Console.ReadLine(), out int folderId);
            if(result == false)
            {
                throw new Exception("Ha habido un error al leer el id.");
            }

            return folderId;
        }

        /// <summary>
        /// Crea un objeto de tipo FileExtension para ser guardado en la base de datos.
        /// </summary>
        /// <param name="folders">Query que tiene las carpetas.</param>
        /// <returns>Retorna el objeto si ingresa un nombre correcto.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileExtension> CreateFileExtensionAsync(IQueryable<Folder> folders)
		{
            Console.Clear();
            if (await folders.AnyAsync() == false)
                throw new Exception("No se han encontrado carpetas. Error al cargar el menú.");

            int folderNumber = 1;
            await folders.ForEachAsync((folder) =>
			{
				Console.WriteLine(string.Format("{0}) Id: {1}. Nombre: {2}.", folderNumber, folder.Id, folder.FolderName));
				folderNumber++;
            });

            Console.Write("Seleccionar id: ");
            var folderResult = int.TryParse(Console.ReadLine(), out int folderId);
            if (folderResult == false)
            {
                throw new Exception("Ha habido un error al leer el id.");
            }

            Console.Clear();
            Console.Write("Nombre de la extensión (.nombre_extension): ");
			string? fileExtensionName = Console.ReadLine() ?? throw new Exception("No se ha ingresado un nombre válido.");

			return new()
			{
				FileExtensionName = fileExtensionName,
				FolderId = folderId
			};
        }

        /// <summary>
        /// Muestra un menú en pantalla y retorna el id de la extensión a eliminar.
        /// </summary>
        /// <param name="folders">Query con los folders y las extensiones incluidas.</param>
        /// <returns>Tarea que retorna el id de la extensión.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> DeleteFileExtensionAsync(IQueryable<Folder> folders)
        {
            Console.Clear();
            if (await folders.AnyAsync() == false)
                throw new Exception("No se han encontrado carpetas. Error al cargar el menú.");

            await folders.ForEachAsync((folder) =>
            {
                Console.WriteLine(string.Format("Carpeta: {0}", folder.FolderName));
                folder.FileExtensions.ForEach((fileExtension) =>
                {
                    Console.WriteLine(string.Format("\t - Id: {0}. Extensión: {1}.", fileExtension.Id, fileExtension.FileExtensionName));
                });
            });

            Console.Write("Ingrese el id de la extensión a eliminar: ");
            bool result = int.TryParse(Console.ReadLine(), out int fileExtensionId);
            if (result == false)
                throw new Exception("Ha habido un error al leer el id.");

            return fileExtensionId;
        }

        /// <summary>
        /// Muestra los datos guardados en pantalla.
        /// </summary>
        /// <param name="folders">Query con los folders y las extensiones incluidas.</param>
        /// <returns>Tarea que retorna void.</returns>
        /// <exception cref="Exception"></exception>
        public async Task ViewDataAsync(IQueryable<Folder> folders)
        {
            Console.Clear();
            if (await folders.AnyAsync() == false)
                throw new Exception("No se han encontrado carpetas. Error al cargar el menú.");

            await folders.ForEachAsync((folder) =>
            {
                Console.WriteLine(string.Format("Carpeta: {0}", folder.FolderName));
                folder.FileExtensions.ForEach((fileExtension) =>
                {
                    Console.WriteLine(string.Format("\t - Id: {0}. Extensión: {1}.", fileExtension.Id, fileExtension.FileExtensionName));
                });
            });

            Console.ReadLine();
        }

        public async Task OrderFolderAsync(IQueryable<Folder> folders, IEnumerable<FileExtension> fileExtensions)
        {
            Console.Clear();
            if (await folders.AnyAsync() == false)
                throw new Exception("No se han encontrado carpetas. Error al cargar el menú.");
            Console.WriteLine("Ingrese la ruta de la carptea que desea organizar:");
            string? pathBase = Console.ReadLine();

            if (string.IsNullOrEmpty(pathBase))
                throw new Exception("Ruta inválida.");
            string[] files = Directory.GetFiles(pathBase);
            if (files.Any() == false)
                throw new Exception("No se han encontrado archivos en la ruta especificada.");

            Console.WriteLine("Organizando archivos...");

            await folders.ForEachAsync((folder) =>
            {
                var path = Path.Combine(pathBase, folder.FolderName);
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
            });

            foreach(var filePath in files)
            {
                string? fileName = Path.GetFileName(filePath);
                if (string.IsNullOrEmpty(fileName))
                    continue;

                string? fileExtension = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExtension))
                    continue;

                var existExtension = fileExtensions.FirstOrDefault((extension) => extension.FileExtensionName == fileExtension);
                if (existExtension == default)
                    continue;

                string? newFolderName = existExtension.Folder?.FolderName;
                if (string.IsNullOrEmpty(newFolderName))
                    continue;

                var newPath = Path.Combine(pathBase, newFolderName, fileName);
                using var memoryStream = new MemoryStream(await File.ReadAllBytesAsync(filePath));

                using var stream = new FileStream(newPath, FileMode.CreateNew);
                await  memoryStream.CopyToAsync(stream);

                File.Delete(filePath);
            }
        }

		public int LastItemNumber { get => _menuOptions.Length; }
	}
}

