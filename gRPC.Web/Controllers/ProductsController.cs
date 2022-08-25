using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gRPC.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ProductCRUD.gRPC;

namespace gRPC.Web.Controllers
{
    
    
    public class ProductsController : Controller
    {
        private readonly gRPCProductService _productService;

        public ProductsController(gRPCProductService productService)
        {
            _productService = productService;
        }

        public async  Task<IActionResult> List()
        {
            var productList = new List<ProductResponse>();
            await foreach (var item in _productService.GetAll())
            {
                productList.Add(item);
            }
            
            return View(productList);
        }

        public async Task<IActionResult> Create()
        {

            await _productService.Create(new ProductCreateRequest()
            {
                CategoryName = "Mouses", Name = "mouse 1",
                Price = 200, Stock = 20, ProductType = ProductType.Large
            });

            return RedirectToAction(nameof(ProductsController.List));

        }

        public async Task<IActionResult> Update()
        {


          await   _productService.Update(new()
            {
                Id = 1, CategoryName = "pennn", Name = "pennn1", Price = 111, Stock = 11,
                ProductType = ProductType.Large
            });


          return RedirectToAction(nameof(ProductsController.List));


        }

        public async Task<IActionResult> Delete()
        {

            await _productService.Delete(new ProductIdRequest() { Id = 1 });
            return RedirectToAction(nameof(ProductsController.List));
        }
    }
}