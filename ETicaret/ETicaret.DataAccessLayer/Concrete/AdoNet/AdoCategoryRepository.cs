using ETicaret.DataAccessLayer.Abstract;
using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Concrete.AdoNet
{
    public class AdoCategoryRepository : ICategoryRepository
    {
        public void Create(Category entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Category entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductFromCategory(int productId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public List<Category> GetAll()
        {
            throw new NotImplementedException();
        }

        public Category GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            throw new NotImplementedException();
        }

        public List<Category> PopulerCategory()
        {
            throw new NotImplementedException();
        }

        public void Update(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}
