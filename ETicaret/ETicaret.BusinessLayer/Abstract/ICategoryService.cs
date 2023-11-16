using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.BusinessLayer.Abstract
{
    public interface ICategoryService
    {
        Category GetById(int id);
        List<Category> GetAll();
        void Create(Category category);
        void Update(Category category);
        void Delete(Category category);
        Category GetByIdWithProducts(int categoryId);
        void DeleteProductFromCategory(int productId, int categoryId);
    }
}
