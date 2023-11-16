using ETicaret.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.WebUI.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        ICategoryService _categoryService;

        public CategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke() 
        { 
            return View(_categoryService.GetAll()); 
        }
    }
}
