using ETicaret.BusinessLayer.Abstract;
using ETicaret.Entities;
using ETicaret.WebUI.Models;
using ETicaret.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.WebUI.Controllers
{
    public class ProductController : Controller
    {
        IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult List(string category, int page = 1)
        {
            int pageSize = 2; // sayfadaki ürün sayısını belirlediğimiz değişken...

            return View(new ProductListViewModel()
            {
                Products = _productService.GetProductsByCategory(category, pageSize, page),
                PageInfo = new PageInfo() { 
                    TotalItems = _productService.GetCountByCategory(category),
                    ItemsPerPage = pageSize,
                    CurrentPage = page,
                    CurrentCategory = category
                }
            });


        }
        //domain/url
        // domain/iphone11
        public IActionResult Details(string url)
        {
            // parametre olarak gönderilen Id'ye ait Product'ı bulun..
            if (url == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(url);
            if (product == null)
            {
                return NotFound();
            }
            ProductDetailModel pdm = new ProductDetailModel()
            {
                Product = product,
                Categories = product.ProductCategories.Select(c => c.Category).ToList()
            };

            return View(pdm);
        }

        public IActionResult Search(string search)
        {
            ProductListViewModel productListViewModel = new ProductListViewModel()
            {
                Products = _productService.GetSearchResult(search)
            };


            return View(productListViewModel);
        }
    }
}
