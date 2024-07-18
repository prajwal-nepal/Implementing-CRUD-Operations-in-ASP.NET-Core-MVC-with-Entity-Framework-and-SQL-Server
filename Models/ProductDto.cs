using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRUD_OPERATION_WEB_APP.Models
{
	public class ProductDto
	{
		public int Id { get; set; }

		[Required, MaxLength(100)]
		public string Name { get; set; } = "";

		[Required, MaxLength(100)]
		public string Brand { get; set; } = "";

		[Required, MaxLength(100)]
		public string Category { get; set; } = "";

		[Required]
		public string Price { get; set; }

		[Required]
		public string Description { get; set; } = "";

		public  IFormFile? ImageFile { get; set; }


	}
}
