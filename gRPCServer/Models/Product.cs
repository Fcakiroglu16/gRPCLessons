namespace gRPCLessons.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Type { get; set; }

    public DateTime Created { get; set; }
    public string CategoryName { get; set; }
}