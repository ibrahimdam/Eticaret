using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Abstract
{
    public interface ICartRepository : IRepository<Cart>
    {
        void ClearCart(int cartId);
        void DeleteFromCart(int cartId, int productId);
        Cart GetCartByUserId(string userId);

    }
}
