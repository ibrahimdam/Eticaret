using ETicaret.DataAccessLayer.Abstract;
using ETicaret.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ETicaretContext>, ICategoryRepository
    {
        public void DeleteProductFromCategory(int productId, int categoryId)
        {
            using(var context = new ETicaretContext())
            {
                // Sql Sorguları yazarak da Ef üzerinden veritabanında SQL'lerin çalıştırılmasını sağalayabiliyoruz.
                var sqlCommand = "delete from ProductCategory where CategoryId = @p0 and ProductId = @p1";
                context.Database.ExecuteSqlRaw(sqlCommand, categoryId, productId);
            }
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            using(var context = new ETicaretContext())
            {
                return context.Categories
                    .Where(c=> c.Id==categoryId)
                    .Include(x=> x.ProductCategories)
                    .ThenInclude(x=> x.Product)
                    .FirstOrDefault();
            }
        }

        public List<Category> PopulerCategory()
        {
            throw new NotImplementedException();
        }
    }
}
