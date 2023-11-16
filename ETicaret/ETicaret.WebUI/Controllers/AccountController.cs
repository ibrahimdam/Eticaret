using ETicaret.BusinessLayer.Abstract;
using ETicaret.WebUI.EmailServices;
using ETicaret.WebUI.Identity;
using ETicaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;

namespace ETicaret.WebUI.Controllers
{
    public class AccountController : Controller
    {
        //5- Account ile ilgili gelen istekleri yöneteceğim Controller burası olacak.
        //İlgili Manager sınıflarını aşağıda tanımlıyorum. Bu sınıflar Identity ile birlikte gelen sınıflardır. Biz tanımlamadık..
        private UserManager<User> _userManager;     // Kullanıcı oluşturma parola oluşturma gibi işleml için kullanılacak olan sınıf.
        private SignInManager<User> _signInManager; // Cookie olaylarını yöneteceğim sınıf..
        // İlgili manager sınıfları için Injection işlemlerini aşağıdaki Constructor'da tanımlıyorum..
        private IEmailSender _emailSender;
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl =null)
        {
            ModelState.Remove("ReturnUrl");
            var model = new LoginModel { ReturnUrl = ReturnUrl };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // user name ve password bilgisi veritabanından sorgulanıp doğrulanacak..
            ModelState.Remove("ReturnUrl");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user==null)
            {
                ModelState.AddModelError("","Kullanıcı adı sistemde kayıtlı değil.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                //_userManager.IsEmailConfirmedAsync(user) kullanıcı hesabına ait email'in confirm edilip edilmediğini kontrol eder. Email hesabı gelen link ile Onaylanmış ise burası true değer döndürür.. Bu durumda kod buraya girmez. Eğer hesap onaylanmamışsa False döndürür. Tersi true olacağı için buraya girer ve kullanıcıya aşağıdaki uyarı mesajı görünür.
                CreateMessage("Lütfen e-posta adresine gönderilen link ile hesabınızı aktif ediniz.", "warning");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            // _signInManager ile kullanıcının tarayıcısına bir Cookie bırakacağız.
            // PasswordSignInAsync 4 farklı parametre alıyor.. ilk parametreye user nesnesini veriyoruz. 2. parametrede de password bilgisini veriyoruz.(modelden aldığımız password bilgisini veriyoruz.) 3. parametre: isPersistent=true tarayıcı kapandığında ya da 30 dakikalık süre bittiğinde cookie silinir. ve otomatik olarak logout işlemi yapılır..
            // lockOutOnFailure=true olursa Program.cs'de verdiğimiz sayı kadar denemede yanlış password girersek hesabı kilitler. False olursa kilitlemez.
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
                //2 soru işareti ile null kontrolü yapılıyor. ReturnUrl null değilse içinde tuttuğu Url'e gidecek. Eğer ReturnUrl null ise ~/ ile root/domain (localhost:7011) adrese gidecek.
            }

            ModelState.AddModelError("","Kullanıcı adı ya da şifre yanlış");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new User()
            {
                FirstName = model.FirstName, 
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // yapılacak işlemler var...
                // token oluştur...
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //url oluştur.
                //Console.WriteLine(code);
                var url = Url.Action("ConfirmEmail","Account",new
                {
                    userId= user.Id,
                    token = code
                });
                //Console.WriteLine("url : " + url);

                // mail gönder
                await _emailSender.SendEmailAsync(user.Email, "E-Ticaret sitesi için üyeliğinizi onaylayın", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:7024{url}'> tıklayınız. </a>");

                return RedirectToAction("Login");
            }
            ModelState.AddModelError("","Bir hata oluştu. Lütfen tekrar deneyiniz.");
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            // Kullanıcıya gönderilen emaildeki link ile emailin confirm edilmesini sağlayacak action burası olacak. 
            if (userId == null || token == null)
            {
                // Ekranda hata mesajı göster
                CreateMessage("Geçersiz userId ve token.", "danger");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    // cart nesnesinin oluşturulması gerekiyor ve veritabanında ilgili datanın eklenmesi gerekiyor. Bunu Hesap onaylandığı zaman yapacağız.çünkü kişinin hesabı onaylandıktan hemen sonra alışveriş için sepete ürün eklemesini istiyoruz.
                    // 
                    _cartService.InitializerCart(user.Id);

                    // hesap onaylandı mesajı verilmesi gerekiyor.
                    CreateMessage("Hesabınız onaylandı.","success");
                    return View();
                }
            }
            // Hesap Onaylanmadı mesajı verilecek.
            CreateMessage("Hesabınız onaylanmadı.", "warning");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Veritabanından ilgili email adresi sorgulanacak. bu emaile ait user var ise 
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var url = Url.Action("ResetPassword","Account", new
                {
                    userId = user.Id,
                    token = code
                });

                await _emailSender.SendEmailAsync(user.Email, "Şifre sıfırlama", $"Şifrenizi yenilemek için linke <a href='https://localhost:7024{url}'> tıklayınız. </a>");

                return RedirectToAction("Login");
            }

            return View();

        }

        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (userId ==null || token==null)
            {
                // bir hata sayfası düzenlenebilir ve userId veya token yanlış şeklinde mesaj verilebilir.
                // Ya da anasayfaya yönelendirilebilir.
                return RedirectToAction("Index", "home");
            }
            var model = new ResetPasswordModel()
            {
                Token = token
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "home");
            }
            // aşağıdaki metot ile passwordüm veritabanında resetlenmiş olacak..
            var result = await _userManager.ResetPasswordAsync(user,model.Token, model.Password );
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }
        [AllowAnonymous]        // Bu durumda Controller seviyesinde bir kısıtlama konsa dahi bu actiona herkes ulaşabilir.
        public IActionResult AccessDenied()
        {
            return View();
        }
        [NonAction] //Acion değil normal bir metot oluyor ve gelen istekleri artık karşılamayacak..
        private void CreateMessage(string message, string alertType)
        {
            var msg = new AlertMessage()
            {
                Message = message,
                AlertType = alertType
            };

            TempData["message"] = JsonConvert.SerializeObject(msg);
        }

       

    }
}
