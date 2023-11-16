namespace ETicaret.WebUI.Models
{
    public class CartModel
    {
        public int CartId { get; set; }
        public List<CartItemModel> CartItems { get; set; }

        public double? TotalPrice()
        {
            // Sepetin toplam fiyatını hesaplıyorum..
            return CartItems.Sum(x=> x.Quantity * x.Price);
        }
    }

    public class CartItemModel
    {
        // Ekranda gösterilecek olan alanları tanımlıyorum...
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName{ get; set; }
        public double? Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public string ProductUrl { get; set; }



    }
}
