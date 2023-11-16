using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string category, int pageSize, int page);
        int GetCountByCategory(string category);
        List<Product> GetSearchResult(string searchString);
        List<Product> GetHomePageProducts();
        Product GetByIdWithCategories(int id);
        void Update(Product product, int[] categoryIds);
    }
}
