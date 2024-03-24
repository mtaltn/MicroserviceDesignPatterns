using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Product.Api.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}
