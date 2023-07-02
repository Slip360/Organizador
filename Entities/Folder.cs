using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Organizador.Entities
{
	/// <summary>
	/// Entidad de las carpetas que guardarán los archivos.
	/// </summary>
	[Index(nameof(FolderName), IsUnique = true)]
	public class Folder
	{
		/// <summary>
		/// Id del registro.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// Nombre del folder.
		/// </summary>
		[Required]
		public string FolderName { get; set; } = string.Empty;

		/// <summary>
		/// Contiene las extensión que admite el folder.
		/// </summary>
		public List<FileExtension> FileExtensions { get; set; } = new();
	}
}
