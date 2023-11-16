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
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ETicaretContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ETicaretContext())
            {
                Product result = context.Products
                        .Where(p=> p.Id==id)
                        .Include(x=> x.ProductCategories)
                        .ThenInclude(x=> x.Category)
                        .FirstOrDefault();

                return result;
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new ETicaretContext())
            {               
                var products = context.Products.Where(x => x.IsApproved == true).AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products                        
                        .Include(i => i.ProductCategories)
                        .ThenInclude(c => c.Category)
                        .Where(p => p.ProductCategories.Any(x => x.Category.Url.ToLower() == category.ToLower()));
                }
                return products.Count();
                
            }
        }

        public List<Product> GetHomePageProducts()
        {
            // IsHome=true olan kayıtlar sorgulanacak.. Ve aynı zamanda IsApproved = true olmalı..
            using (var context = new ETicaretContext())
            {
                  return context.Products.Where(x=> x.IsHome == true && x.IsApproved == true).ToList();          
            }

        }

        // Product'a özgü database işlemleri yapılacaksa ilgili metodun imzasını önce IProductRepository tanımlıyorum daha sonra içini de burada dolduruyorum.
        public Product GetProductDetails(string url)
        {
            using (var context = new ETicaretContext())
            {
                return context.Products
                    .Where(p => p.Url == url)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(c => c.Category)
                    .FirstOrDefault();
            }

        }

        public List<Product> GetProductsByCategory(string category, int pageSize, int page)
        {
            using (var context = new ETicaretContext())
            {
                // category null olabilir. Eğer null ise bütün ürünler geriye gönderilecek. Eğer category null değilse bu durumda veritabanından sorgulama bu category'e göre yapılmalı...
                var products = context.Products.Where(x => x.IsApproved == true).AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(c => c.Category)
                        .Where(p => p.ProductCategories.Any(x => x.Category.Url.ToLower() == category.ToLower()));
                }
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                // Skip'e verilen değer kadar kayıt alınmıyor.
                // Take ile de verilen değer kadar kayıt tablodan okunuyor.
            }
        }

        public List<Product> GetSearchResult(string searchString)
        {
            // Kullanıcı bir ürünü aramak istediğin yazmış olduğu string ifadeyi 2 farklı alanda arama yapabiliriz. 1- Product.name alanı ve Product.Description alanı. Buna göre gelen sonuçları ilgili sayfaya göndeririz.
            // Diğer koşulumuz da ürünün onaylanmış olması yani IsApproved alanının true olması..
            // searchString null olarak gelirse, bir exception oluşacak ve sorgu patlayacak. Bu durumda 2 farklı sorgu oluşturup sonucu geriye döndürmeliyiz. yani searchStringin null olduğu durum ve null olmadığı durum...
            using (var context = new ETicaretContext())
            {
                // TODO: i, I sorunu var, çözülmesi gerek.

               List<Product> products;
                if (string.IsNullOrEmpty(searchString))
                {
                    // searchString null geldiyse buradaki kodlar çalışacak.
                    products = context.Products.Where(p=> p.IsApproved==true).ToList();
                }
                else
                {
                    // searchString dolu(veri ile) geldiyse buradaki kodlar çalışacak.
                    products = context.Products.Where(p => p.IsApproved == true && (p.Name.ToLower().Contains(searchString.ToLower().Trim()) || p.Description.ToLower().Contains(searchString.ToLower().Trim())))
                        .ToList();
                }
                return products;
            }
        }

        public void Update(Product product, int[] categoryIds)
        {
            using(var context = new ETicaretContext())
            {
                //Öncelikle veritabanından ilgili kaydı bulacağım(product nesnesini ProductCategory verisi ile sorugulamam gerekiyor.), daha sonra parametreden gelen değerleri, gelen veriye aktarıp update işlemini yaptıracağım.

                var entity = context.Products
                    .Include(p=> p.ProductCategories)
                    .FirstOrDefault(p=> p.Id == product.Id);

                if (entity != null)
                {
                    entity.Name = product.Name;
                    entity.Description = product.Description;
                    entity.Price = product.Price;
                    entity.Url = product.Url;
                    entity.ImageUrl = product.ImageUrl;
                    entity.IsApproved = product.IsApproved;
                    entity.IsHome = product.IsHome;

                    entity.ProductCategories = categoryIds.Select(catId => new ProductCategory() {
                        ProductId = product.Id,
                        CategoryId = catId
                        }).ToList();
                    context.SaveChanges();
                }
            }
        }
    }
}
