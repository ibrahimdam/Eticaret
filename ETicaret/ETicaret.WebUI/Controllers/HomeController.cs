using ETicaret.BusinessLayer.Abstract;
using ETicaret.DataAccessLayer.Abstract;
using ETicaret.DataAccessLayer.Concrete.AdoNet;
using ETicaret.DataAccessLayer.Concrete.EfCore;
using ETicaret.Entities;
using ETicaret.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ETicaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        ICategoryService _categoryService;
        IProductService _productService;

        public HomeController(ICategoryService categoryService, IProductService productService)
        {
            this._categoryService = categoryService;
            this._productService = productService;

        }

        public IActionResult Index()
        {
           return View( new ProductListViewModel()
           {
               Products = _productService.GetHomePageProducts()
           });
        }

        public IActionResult About()
        {
            return View();

        }

        public IActionResult Contact()
        {
            return View();

        }

    }
}