using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPCLessons.Extensions;
using gRPCLessons.Models;
using Microsoft.EntityFrameworkCore;
using ProductCRUD.gRPC;

namespace gRPCLessons.Services;

public class ProductsGrpcService : ProductService.ProductServiceBase
{
    private readonly AppDbContext _context;

    public ProductsGrpcService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task Create(IAsyncStreamReader<ProductCreateRequest> requestStream,
        IServerStreamWriter<ProductCreateResponse> responseStream, ServerCallContext context)
    {
        List<ProductCreateRequest> productCreateRequestList = new();

        await foreach (var product in requestStream.ReadAllAsync()) productCreateRequestList.Add(product);

        var createProductList = productCreateRequestList.ConverToProduct().ToList();

        await _context.Products.AddRangeAsync(createProductList);

        await _context.SaveChangesAsync();

        foreach (var createdProduct in createProductList)
            await responseStream.WriteAsync(createdProduct.ConverToProductCreateResponse());
    }

    public override async Task GetAll(Empty request, IServerStreamWriter<ProductResponse> responseStream,
        ServerCallContext context)
    {
        var products = await _context.Products.ConverToProductResponse().ToListAsync();

        foreach (var productResponse in products)
        {
            await responseStream.WriteAsync(productResponse);
            await Task.Delay(2000);
        }
    }

    public override async Task<ProductResponse> GetById(ProductIdRequest request, ServerCallContext context)
    {
        var product = await _context.Products.ConverToProductResponse().FirstOrDefaultAsync(x => x.Id == request.Id);

        if (product == null) throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

        return product;
    }

    public override async Task<Empty> Delete(IAsyncStreamReader<ProductIdRequest> requestStream,
        ServerCallContext context)
    {
        var productList = new List<ProductIdRequest>();

        await foreach (var item in requestStream.ReadAllAsync()) productList.Add(item);

        _context.Products.RemoveRange(productList.Select(x => new Product { Id = x.Id }));
        await _context.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<Empty> Update(ProductUpdateRequest request, ServerCallContext context)
    {
        var updateProduct = request.ConverToProduct();

        _context.Products.Update(updateProduct);
        _context.Entry(updateProduct).Property(x => x.Created).IsModified = false;
        await _context.SaveChangesAsync();
        return new Empty();
    }
}