using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using ProductCRUD.gRPC;

namespace gRPC.Web.Services;

public class gRPCProductService
{
    private readonly ProductService.ProductServiceClient _client;

    public gRPCProductService(ProductService.ProductServiceClient client)
    {
        _client = client;
    }


    public async IAsyncEnumerable<ProductResponse> GetAll()
    {
        using var asyncServerStreamingCall = _client.GetAll(new Empty());
        while (await  asyncServerStreamingCall.ResponseStream.MoveNext(CancellationToken.None))
        {
            var result = asyncServerStreamingCall.ResponseStream.Current;

            yield return result;

        }
    }

    public async Task<List<ProductCreateResponse>> Create(ProductCreateRequest request)
    {
        var bidirectionalStream = _client.Create(new CallOptions());
        
      await bidirectionalStream.RequestStream.WriteAsync(request);

      await bidirectionalStream.RequestStream.CompleteAsync();

 var productResponseList= new List<ProductCreateResponse>(); 
      await foreach (var item in bidirectionalStream.ResponseStream.ReadAllAsync())
      {
       productResponseList.Add(item);
      }

      return productResponseList;

    }


    public async Task Update(ProductUpdateRequest request)
    {
        await _client.UpdateAsync(request);
    }

    public async Task Delete(ProductIdRequest request)
    {
        var clientStream= _client.Delete();

       await clientStream.RequestStream.WriteAsync(request);

       await clientStream.RequestStream.CompleteAsync();
       await  clientStream.ResponseAsync;
 
    }

}