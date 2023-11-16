using ETicaret.Entities;

namespace ETicaret.WebUI.ViewModels
{
    public class ProductListViewModel
    {
        // Burası  Product datalarını View tarafında listelerken kullanacağım bir class olacak. İlerleyen aşamalarda sayfalama yapısını kuracağımız zaman da bu class'tan faydalanacağız. 

        public List<Product> Products { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}
