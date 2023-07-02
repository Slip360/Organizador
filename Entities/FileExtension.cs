using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Organizador.Entities
{
	/// <summary>
	/// Extensiones que puede guardar una carpeta.
	/// </summary>
	[Index(nameof(FileExtensionName), IsUnique = true)]
	public class FileExtension
	{
		/// <summary>
		/// Id del registro.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// Nombre de la extensión del archivo.
		/// </summary>
		[Required]
		[RegularExpression("^.[a-z]+")]
		public string FileExtensionName { get; set; } = "";

		/// <summary>
		/// Propiedad de acceso con el carpeta.
		/// </summary>
		public Folder? Folder { get; set; } = null;

		/// <summary>
		/// Id del folder
		/// </summary>
		[Required]
		public int FolderId { get; set; }
	}
}
