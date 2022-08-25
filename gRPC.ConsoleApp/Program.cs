// See https://aka.ms/new-console-template for more information

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductCRUD.gRPC;


var channel = GrpcChannel.ForAddress("http://localhost:5000");
var  productService=  new ProductService.ProductServiceClient(channel);

await GetById(productService,1);
await CreateProduct(productService);
await UpdateProduct(productService);
await GetProductList(productService);
await DeleteProduct(productService);

async Task GetProductList(ProductService.ProductServiceClient 
productServiceClient)
{
    using var asyncServerStreamingCall = productServiceClient.GetAll(new Empty());

    await foreach (var item in asyncServerStreamingCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"GetProductList {item.Id} {item.Name}");
    }
    
    var header = await asyncServerStreamingCall.ResponseHeadersAsync;
    var trailer = asyncServerStreamingCall.GetTrailers();

   
}

async Task CreateProduct(ProductService.ProductServiceClient productService1)
{
    using var createServerClientStream = productService1.Create(new CallOptions());

    await createServerClientStream.RequestStream.WriteAsync(new ProductCreateRequest()
        { Name = "Pen 50", Price = 50, ProductType = ProductType.Large, Stock = 5, CategoryName = "Pens" });

    await createServerClientStream.RequestStream.CompleteAsync();

    await foreach (var item in createServerClientStream.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"New Product Id :{item.Id}");
    }
}

async Task GetById(ProductService.ProductServiceClient productServiceClient1,int Id)
{
    var getByIdUnaryStreamByProduct = await productServiceClient1.GetByIdAsync(new ProductIdRequest() { Id = Id });

    Console.WriteLine($"GetById :{getByIdUnaryStreamByProduct.Id} {getByIdUnaryStreamByProduct.Name}");
 
}

async Task UpdateProduct(ProductService.ProductServiceClient productService2)
{
    await productService2.UpdateAsync(new ProductUpdateRequest()
        { Id = 1, Name = "50001", Price = 5000, CategoryName = "Pens", Stock = 5, ProductType = ProductType.Large });
}

async Task DeleteProduct(ProductService.ProductServiceClient productServiceClient2)
{
    var deleteRequestStream = productServiceClient2.Delete(new CallOptions());
    await deleteRequestStream.RequestStream.WriteAsync(new ProductIdRequest() { Id = 1 });
    await deleteRequestStream.RequestStream.CompleteAsync();
    var emptyResult = await deleteRequestStream.ResponseAsync;
    var status = deleteRequestStream.GetStatus();
}
