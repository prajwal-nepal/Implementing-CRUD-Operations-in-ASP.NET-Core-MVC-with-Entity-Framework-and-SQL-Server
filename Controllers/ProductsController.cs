using CRUD_OPERATION_WEB_APP.Models;
using CRUD_OPERATION_WEB_APP.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_OPERATION_WEB_APP.Controllers
{
	public class ProductsController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;

		public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
		{
			this.context = context;
			this.environment = environment;
		}
		public IActionResult Index()
		{
			var products = context.Products.ToList();
			return View(products);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(ProductDto productDto)
		{
			if (productDto.ImageFile == null)
			{
				ModelState.AddModelError("ImageFile", "The image file is required");
			}

			if (!ModelState.IsValid)
			{
				return View(productDto);
			}

			// Generate a unique filename for the image
			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssff") + Path.GetExtension(productDto.ImageFile.FileName);

			// Define the directory path where images will be saved
			string imageDirectoryPath = Path.Combine(environment.WebRootPath, "products");

			// Combine the directory path with the new filename to get the full path
			string imageFullPath = Path.Combine(imageDirectoryPath, newFileName);

			// Create a FileStream to save the uploaded image file
			using (var stream = new FileStream(imageFullPath, FileMode.Create))
			{
				await productDto.ImageFile.CopyToAsync(stream);
			}


			//for saving the new product in the database
			Product product = new Product()
			{
				Name = productDto.Name,
				Brand = productDto.Brand,
				Category = productDto.Category,
				Price = productDto.Price,
				Description = productDto.Description,
				ImageFileName = imageFullPath,

			};

			context.Products.Add(product);
            await context.SaveChangesAsync();



            return RedirectToAction("Index", "Products");


		}


		public IActionResult Edit(int id)
		{
			var product = context.Products.Find(id);

			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}

			// creating the productDto from product
			var productDto = new ProductDto()
			{
				Name = product.Name,
				Brand = product.Brand,
				Category = product.Category,
				Price = product.Price,
				Description = product.Description,
			};


			ViewData["Productid"] = product.Id;
			ViewData["ImageFileName"] = product.ImageFileName;




			return View(productDto);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, ProductDto productDto)
		{
			var product = context.Products.Find(id);

			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}

			if (!ModelState.IsValid)
			{
				ViewData["ProductId"] = product.Id;
				ViewData["ImageFileName"] = product.ImageFileName;

				return View(productDto);
			}

			// Update the image file if a new image file is uploaded
			string newFileName = product.ImageFileName;
			if (productDto.ImageFile != null)
			{
				newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				newFileName += Path.GetExtension(productDto.ImageFile.FileName);

				string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);
				using (var stream = new FileStream(imageFullPath, FileMode.Create))
				{
					await productDto.ImageFile.CopyToAsync(stream);
				}

				// Delete the old image
				string oldImageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);
				if (System.IO.File.Exists(oldImageFullPath))
				{
					System.IO.File.Delete(oldImageFullPath);
				}
			}

			// Update the product in the database
			product.Name = productDto.Name;
			product.Brand = productDto.Brand;
			product.Category = productDto.Category;
			product.Price = productDto.Price;
			product.Description = productDto.Description;
			product.ImageFileName = newFileName;

			context.SaveChanges();

			return RedirectToAction("Index", "Products");
		}

		public IActionResult Delete(int id)
		{
			var product = context.Products.Find(id);
			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}
			string imageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);
			System.IO.File.Delete(imageFullPath);

			context.Products.Remove(product);
			context.SaveChanges(true);
			return RedirectToAction("Index", "Products");
		}


	}
}



