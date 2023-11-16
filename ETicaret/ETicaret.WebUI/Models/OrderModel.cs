namespace ETicaret.WebUI.Models
{
    public class OrderModel
    {
        // Sonu Model ile biten Classlarımız Kullanıcıya veri göstermek ya da kullanıcıdan veri almak için kullandığımız classlarımızdır. Buradaki alanlar da hangi bilgileri gösterecek ya da alacaksak onlar ile ilgili...
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string CardName { get; set; }
        public string CardNumber{ get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string Cvc { get; set; }
        public CartModel CartModel { get; set; }

        
    }
}
