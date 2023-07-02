using Organizador.Configurations;
using Organizador.Context;

// Verificando que la base de datos y la carpeta existan.
string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase");
var factory = new ApplicationDbContextFactory(directoryPath);
Directory.CreateDirectory(directoryPath);

bool exist = Directory.Exists(directoryPath);
if(exist)
{
    using var dbContext = factory.CreateDbContext(args);
    await dbContext.Database.EnsureCreatedAsync();
}

// Inicializando los servicios.
var menu = new MenuService();

// Almacenador de servicios.
List<IDisposableAsync> services = new();

// Manejar la lógica del sistema
while (true)
{
    // Manejador de errores.
    try
    {
        // Instanciamos los servicios.
        FolderService folderService = new(factory.CreateDbContext(args));
        FileExtensionService fileExtensionService = new(factory.CreateDbContext(args));

        // Agregamos los servicios que serán desechados.
        services.Add(folderService);
        services.Add(fileExtensionService);

        // Pintando el menu
        menu.PrintMenu();

        bool result = int.TryParse(Console.ReadLine(), out int optionNumber);
        if (result == false || optionNumber == menu.LastItemNumber)
        {
            await Parallel.ForEachAsync(services, async (service, token) =>
            {
                if(service.IsDisposed == false)
                    await service.DisposeAsync();
            });
            break;
        }
        else if (optionNumber == 1)
        {
            var folder = menu.CreateFolder();
            Console.WriteLine("Guardando folder...");
            if (folder != null)
            {
                await folderService.CreatedFolderAsync(folder);
            }
        }
        else if(optionNumber == 2)
        {
            var fileExtension = await menu.CreateFileExtensionAsync(folderService.GetFoldersAsQuery());
            Console.WriteLine("Guardando extensión...");
            await fileExtensionService.CreateFileExtensionAsync(fileExtension);
        }
        else if(optionNumber == 3)
        {
            var folderId = await menu.DeleteFolderAsync(folderService.GetFoldersAsQuery());
            Console.WriteLine("Eliminando folder...");
            await folderService.DeleteFolderAsync(folderId);
        }
        else if(optionNumber == 4)
        {
            var fileExtensionId = await menu.DeleteFileExtensionAsync(folderService.GetFoldersWithFileExtensionsAsQuery());
            Console.WriteLine("Eliminando extensión...");
            await fileExtensionService.DeleteFileExtensionAsync(fileExtensionId);
        }
        else if(optionNumber == 5)
        {
            await menu.ViewDataAsync(folderService.GetFoldersWithFileExtensionsAsQuery());
        }
        else if(optionNumber == 6)
        {
            var fileExtensions = await fileExtensionService.GetFileExtensionsWithFolderAsync();
            await menu.OrderFolderAsync(folderService.GetFoldersAsQuery(), fileExtensions);
        }
        else
        {
            await Parallel.ForEachAsync(services, async (service, token) =>
            {
                if (service.IsDisposed == false)
                    await service.DisposeAsync();
            });
        }

        await Parallel.ForEachAsync(services, async (service, token) =>
        {
            if(service.IsDisposed == false)
                await service.DisposeAsync();
        });
    }
    catch (Exception e)
    {
        await Parallel.ForEachAsync(services, async (service, token) =>
        {
            if(service.IsDisposed == false)
                await service.DisposeAsync();
        });

        Console.Clear();
        Console.WriteLine(e.Message ?? e.InnerException?.Message ?? "Ha habido un error al ejecutar la instrucción.");
        Thread.Sleep(2000);
    }
}
