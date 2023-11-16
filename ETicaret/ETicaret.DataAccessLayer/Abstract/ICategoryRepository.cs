using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void DeleteProductFromCategory(int productId, int categoryId);
        Category GetByIdWithProducts(int categoryId);

        // Category'e özgü yani diğer Entity'leri ilgilendirmeyen işlemler için kullanılacak metot ya da property tanımlamaları yapılır.
        List<Category> PopulerCategory();

    }
}
