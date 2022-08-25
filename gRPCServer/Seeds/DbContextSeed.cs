using gRPCLessons.Models;

namespace gRPCLessons.Seeds;

public class DbContextSeed
{
    public static async Task Seed(AppDbContext context)
    {
        await context.Products.AddAsync(new Product
        {
            Name = "Pen 1", Price = 100, Created = DateTime.Now, CategoryName =
                "Pens",
            Type = 1
        });

        await context.Products.AddAsync(new Product
        {
            Name = "Pen 2", Price = 100, Created = DateTime.Now, CategoryName =
                "Pens",
            Type = 1
        });
        await context.Products.AddAsync(new Product
        {
            Name = "Pen 3", Price = 100, Created = DateTime.Now, CategoryName =
                "Pens",
            Type = 1
        });
        await context.SaveChangesAsync();
    }
}