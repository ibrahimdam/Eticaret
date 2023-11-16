using ETicaret.BusinessLayer.Abstract;
using ETicaret.Entities;
using ETicaret.WebUI.Identity;
using ETicaret.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ETicaret.WebUI.Controllers
{
    [Authorize(Roles ="Customer")]
    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<User> _userManager;
        private IOrderService _orderService;
        public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderService)
        {
            _cartService= cartService;  
            _userManager= userManager;
            _orderService= orderService;
        }

        public IActionResult Index()
        {
            // 
            var userId = _userManager.GetUserId(User);
            var cart = _cartService.GetCartByUserId(userId);


            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x=> new CartItemModel()
                {
                    CartItemId= x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity,
                    ProductUrl = x.Product.Url
                }).ToList(),
            });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                _cartService.AddToCart(userId, productId, quantity);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        { 
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId, productId);
            return RedirectToAction("Index"); 
        }

        public IActionResult CompleteShopping()
        {
            var cart= _cartService.GetCartByUserId(_userManager.GetUserId(User));
            OrderModel model = new OrderModel();
            model.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x=> new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl
                    }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult CompleteShopping(OrderModel model)
        {
            ModelState.Remove("CartModel");
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cart = _cartService.GetCartByUserId(userId);

                model.CartModel = new CartModel()
                {
                    CartId= cart.Id,
                    CartItems = cart.CartItems.Select(i=> new CartItemModel()
                    {
                      CartItemId = i.Id,
                      ProductName = i.Product.Name,
                      Price = i.Product.Price,
                      ImageUrl= i.Product.ImageUrl,
                      ProductId= i.ProductId,
                      Quantity = i.Quantity,
                      ProductUrl = i.Product.Url                      
                    }).ToList()
                };

                var payment = PaymentProcess(model);
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    ClearCart(model.CartModel.CartId);
                    return View("Success");
                }
                else
                {
                    var msg = new AlertMessage() 
                    {
                        Message= $"{payment.ErrorMessage}",
                        AlertType = "danger"
                    };
                    TempData["message"] = JsonConvert.SerializeObject(msg);
                }
            }

            return View(model);
        }
        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-cPvdXr7mJuGz9Li8TfuxEDARhVCj7WIz";
            options.SecretKey = "sandbox-SfZS9L201BaQUu00WYABPa2jrogS4InY";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(100000000,999999999).ToString();
            request.Price = model.CartModel.TotalPrice().ToString();
            request.PaidPrice = model.CartModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.Address;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = $"{model.FirstName} {model.LastName}";
            shippingAddress.City = model.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = model.Address;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;
            request.BillingAddress = shippingAddress;
            //Address billingAddress = new Address();
            //billingAddress.ContactName = "Jane Doe";
            //billingAddress.City = "Istanbul";
            //billingAddress.Country = "Turkey";
            //billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            //billingAddress.ZipCode = "34742";
            //request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            foreach (var item in model.CartModel.CartItems)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.ProductName;
                basketItem.Category1 = item.ProductName;
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Price = (item.Price * item.Quantity).ToString();
                basketItems.Add(basketItem);
            }
                        
            request.BasketItems = basketItems;

            Payment payment = Payment.Create(request, options);
            return payment;
        }


        private void ClearCart(int cartId)
        {
           _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            // Sepete eklediğimiz ürünleri, ödeme işlemi tamalandıktan sonra Carts/CartItems içindeki bilgiler Orders/OrderItems tablosuna taşınmalıdır.
            Order order = new Order();

            order.UserId = userId;
            order.OrderState = EnumOrderState.completed;
            order.PaymentType = EnumPaymentType.CreditCard;
            order.PaymentId = payment.PaymentId;
            order.OrderNumber = new Random().Next(100000, 999999).ToString();
            order.OrderDate = DateTime.Now;
            order.FirstName = model.FirstName;
            order.LastName = model.LastName;
            order.Address = model.Address;
            order.City = model.City;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.Note = model.Note;
            order.ConversationId = payment.ConversationId;

            order.OrderItems = new List<Entities.OrderItem>();

            foreach(var item in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = (double)item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderItem);
            }

            _orderService.Create(order);
        }

        
    }
}
