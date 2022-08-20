using Google.Protobuf.WellKnownTypes;
using gRPCLessons.Models;
using ProductCRUD.gRPC;

namespace gRPCLessons.Extensions;

public static class LinqExtensions
{
    public static ProductCreateResponse ConverToProductCreateResponse(this Product product)
    {
        return new ProductCreateResponse { Id = product.Id };
    }

    public static IEnumerable<Product> ConverToProduct(this IEnumerable<ProductCreateRequest> list)
    {
        return list.Select(x => new Product
        {
            Name = x.Name,
            Price = Convert.ToDecimal(x.Price),
            CategoryName = x.CategoryName,
            Created = DateTime.Now,
            Type = x.ProductType.GetHashCode()
        });
    }

    public static Product ConverToProduct(this ProductUpdateRequest productUpdateRequest)
    {
        return new Product
        {
            Id = productUpdateRequest.Id,
            Name = productUpdateRequest.Name,
            Price = Convert.ToDecimal(productUpdateRequest.Price),
            CategoryName = productUpdateRequest.CategoryName,
            Type = productUpdateRequest.ProductType.GetHashCode()
        };
    }

    public static IQueryable<ProductResponse> ConverToProductResponse(this IQueryable<Product> list)
    {
        return list.Select(product => new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = Convert.ToDouble(product.Price),
            CategoryName = product.CategoryName,
            ProductType = (ProductType)product.Type,
            CreatedDate = product.Created.ToUniversalTime().ToTimestamp()
        });
    }
}