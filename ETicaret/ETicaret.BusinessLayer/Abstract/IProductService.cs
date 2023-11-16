using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.BusinessLayer.Abstract
{
    public interface IProductService
    {
        Product GetById(int id);
        List<Product> GetAll();
        void Create(Product product);
        void Update(Product product);
        void Delete(Product product);
        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string category, int pageSize, int page);
        int GetCountByCategory(string category);
        List<Product> GetSearchResult(string searchString);
        List<Product> GetHomePageProducts();
        Product GetByIdWithCategories(int id);
        void Update(Product product, int[] categoryIds);
    }
}

// metotlarda overload
