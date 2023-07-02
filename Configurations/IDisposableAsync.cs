namespace Organizador.Configurations
{
	/// <summary>
	/// Interfaz que se encarga de hacer un "ApplicationDbContext.Dispose()" de los servicios que la implementen.
	/// </summary>
	public interface IDisposableAsync
	{
		/// <summary>
		/// True si ha sido desechado y false en caso contrario.
		/// </summary>
		bool IsDisposed { get; set; }

		/// <summary>
		/// Hace un Dispose del DbContext.
		/// </summary>
		/// <returns>Tarea que retorna void.</returns>
		Task DisposeAsync();
	}
}

