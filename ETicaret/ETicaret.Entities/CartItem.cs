using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Entities
{
    public class CartItem
    {
        //Sepet detayı
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }

        // ilişkiler
        public Product Product { get; set; }
        public Cart Cart { get; set; }

    }
}
