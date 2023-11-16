namespace ETicaret.WebUI.ViewModels
{
    public class PageInfo
    {
        public int TotalItems { get; set; } //Varitabanında kaç ürün var?
        public int ItemsPerPage { get; set; } // Her sayfada kaç tane ürün göstereceğiz?
        public int CurrentPage { get; set; } // hangi sayfadayım? link verdiğimiz yerde ilgili sayfanın farklı bir renk olmasını sağlamak için kullanacağım.

        public string CurrentCategory { get; set; } // Kategori bilgisi varsa bu bilgi burada tutulacak.

        public int TotalPage()
        {
            int total = (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
            return total;   
            
            
            //10/3=3,3.. decimal değer üretir. bu yüzden decimal'a cast ediyorum... Math.Ceiling ile yukarıya yuvarlıyorum.. sonra decimal olan bu sayıyı da int'e cast ediyorum. 10/3 = 4
        }


    }
}
