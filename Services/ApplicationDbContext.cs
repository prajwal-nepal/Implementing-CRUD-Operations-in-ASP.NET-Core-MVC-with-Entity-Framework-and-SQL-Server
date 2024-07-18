using CRUD_OPERATION_WEB_APP.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_OPERATION_WEB_APP.Services
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)

    {
		public DbSet<Product> Products { get; set; }
    }
}
