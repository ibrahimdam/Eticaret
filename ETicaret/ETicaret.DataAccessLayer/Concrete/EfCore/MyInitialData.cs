using ETicaret.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Concrete.EfCore
{
    public class MyInitialData
    {
        private static Category[] Categories = new Category[]
        {
            new Category() {Name = "Telefon",Url="telefon"},
            new Category() {Name = "Bilgisayar",Url="bilgisayar"},
            new Category() {Name = "Elektronik", Url = "elektronik"},
            new Category() {Name = "Giyim", Url = "giyim"},
            new Category() {Name = "Televizyon", Url = "televizyon"},
            new Category() {Name = "Beyaz Eşya", Url = "beyaz-esya"},
            new Category() {Name = "Küçük Ev Aletleri", Url = "kucuk-ev-aletleri"}
        };
        private static Product[] Products = new Product[]
        {
            new Product() { Name="Iphone 11", Url="iphone11", Description="Iphone serisi", ImageUrl="iphone11.jpg", Price=1500, IsApproved=true, IsHome=false},
            new Product() { Name="Iphone 12", Url="iphone12", Description="Iphone serisi", ImageUrl="iphone12.jpg", Price=1800, IsApproved=true, IsHome=false},
            new Product() { Name="Iphone 13", Url="iphone13", Description="Iphone serisi", ImageUrl="iphone13.jpg", Price=2000, IsApproved=true, IsHome = true},
            new Product() { Name="Asus", Url="asus", Description="Notebook bilgisayar", ImageUrl="asus.jpg", Price=2500, IsApproved=true, IsHome=false},
            new Product() { Name="Toshiba A50", Url="toshiba-a50", Description="Notebook bilgisayar", ImageUrl="toshibaa50.jpg", Price=2600, IsApproved=true, IsHome = true},
            new Product() { Name="LG Tv", Url="lg-tv", Description="Televizyon", ImageUrl="lgtv.jpg", Price=2600, IsApproved=true, IsHome = true},
            new Product() { Name="Gömlek", Url="gomlek", Description="Gömlek", ImageUrl="gomlek.jpg", Price=2600, IsApproved=true, IsHome = true}
        };
        private static ProductCategory[] productCategories = new ProductCategory[]
        {
            new ProductCategory(){Product = Products[0], Category = Categories[0] },
            new ProductCategory(){Product = Products[1], Category = Categories[0] },
            new ProductCategory(){Product = Products[2], Category = Categories[0] },
            new ProductCategory(){Product = Products[3], Category = Categories[1] },
            new ProductCategory(){Product = Products[4], Category = Categories[1] },
            new ProductCategory(){Product = Products[5], Category = Categories[2] },
            new ProductCategory(){Product = Products[6], Category = Categories[3] }
        };
        public static void Seed()
        {
            ETicaretContext context = new ETicaretContext();
            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context.Categories.Count() == 0)
                {
                    foreach (var cat in Categories)
                    {
                        context.Add(cat);
                    }
                }
                if (context.Products.Count() == 0)
                {
                    foreach (var product in Products)
                    {
                        context.Add(product);
                    }
                    foreach (var prodCat in productCategories)
                    {
                        context.Add(prodCat);
                    }
                }
            }

            context.SaveChanges();
        }

    }
}


//https://codeshare.io/8p3KkD