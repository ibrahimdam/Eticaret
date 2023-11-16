using ETicaret.BusinessLayer.Abstract;
using ETicaret.BusinessLayer.Concrete;
using ETicaret.DataAccessLayer.Abstract;
using ETicaret.DataAccessLayer.Concrete.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;
using ETicaret.WebUI.Identity;
using ETicaret.WebUI.EmailServices;

var builder = WebApplication.CreateBuilder(args);

//3- Identity tanýmlamalarý:
builder.Services.AddDbContext<ApplicationContext>(option => option.UseSqlServer("Server=DESKTOP-MCJ9JKH;Database=ETicaret;Integrated Security=true"));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();


//4- Veritabaný tablolarýný oluþturduktan sonra Identity ile ilgili bir takým özelliklerin konfigürasyonunu aþaðýdaki gibi yapabiliriz.
builder.Services.Configure<IdentityOptions>(options =>
{
    // password
    options.Password.RequireDigit = true;   // password^de mutlaka sayýsal bir deðer olmalý.
    options.Password.RequireLowercase = true; //password'de mutlaka küçük harf olmalý.
    options.Password.RequireUppercase = true;   // password'de mutlaka büyük harf olmalý.
    options.Password.RequiredLength = 6;    // password en az 6 karakter olmalý.
    options.Password.RequireNonAlphanumeric = true; // rakam ve harf dýþýnda farklý bir karakterin de password içinde olmasý gerekiyor. Örn: nokta gibi, @ gibi, %,-,_ gibi karakterler...

    // lockout : Kullanýcý hesabýnýn klilitlenip kilitlenmemsi ile ilgili.
    options.Lockout.MaxFailedAccessAttempts = 5;    // yanlýþ parolayý 5 kere girilebilir. Sonra hesap kilitlenir.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Hesap kilitlendikten 5 dakika sonra kullanýcý giriþ yapmayý deneyebilir.

    //User
    options.User.RequireUniqueEmail=true;   // her kullanýcý tek bir email adresi ile sisteme girebilir. Yani uniq email kullanýlýr. Ayný email ile 2 hesap açýlamaz.
    options.SignIn.RequireConfirmedEmail = false; // true olursa kullanýcý üye olur fakat email'ini mutlaka onaylamasý gerekir. false olursa üye olup hemen sisteme girebilir.
    options.SignIn.RequireConfirmedPhoneNumber = false; // True olursa telefon bilgisi için onay ister

});

// 4-1: Cookie ayarlarý: Cookie (Çerez): Kullanýcýnýn tarayýcýsýna býrakýlan bir bilgi diyebiliriz kýsaca. 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath= "/Account/Login";   // sisteme login olmadýysak bizi login sayfasýna yönlendiriyor. login olduysak da bize eþsiz bir sayý üretiyor.. Bu sayýyý server tarafýnda session'da tutuluyor, Cilent tarafýnda ise cookie içinde tutluyor.. Kullanýcý bir iþlem yaptýktan sonra belirli bir süre sonunda bu bilgi siliniyor. Belirtilen özelliklere göre, bu veri belirtilen süre içinde tekrar bir iþlem yapýlýrsa, tekrar login olmamýza gerek kalmýyor. Fakat süre bittikten sonra tekrar login olmamýz gerekiyor.
    options.LogoutPath= "/Account/Logout";  //Çýkýþ iþlemi yaptýðýmda cookie tarayýcýdan silinecek Ve tekrar bir iþlem yapmak istediðimde login sayfasýna yönlendirileceðim.
    options.AccessDeniedPath= "/Account/Accessdenied"; // Yetkisiz iþlem yapýldýðýnda çalýþacak olan Action.. Örneðin sýradan bir kullanýcý Admin ile ilgili bir sayfaya ulaþmaya çalýþtýðýnda çalýþacak.

    options.SlidingExpiration = true; // Örneðin sisteme girdim iþlem yaptým ve bekledim. varsayýlan deðer 20dakika. 20dakikadan sonra cookie'de bu bilgi silenecek. Eðer 20 dakika içinde tekrardan bir iþlem yaparsam bu süre tekrardan 20 dakika olarak ayarlanacak. False olursa login olduktan sonra 20 dakika sonunda cookie silinecektir.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(300); // default süresi 20dakika..

    options.Cookie = new CookieBuilder
    { HttpOnly = true, Name=".ETicaret.Security.Cookie"};
    // HttpOnly = true sadece http ile istek geldiðinde ulaþýlabilir olsun diyoruz.
    // Name propertry'si ile de Cookie'ye özel bir isim verebiliyoruz.
});




//IoC Container
builder.Services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

builder.Services.AddScoped<IProductRepository, EfCoreProductRepository>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddScoped<ICartRepository, EfCoreCartRepository>();
builder.Services.AddScoped<ICartService, CartManager>();

builder.Services.AddScoped<IOrderRepository, EfCoreOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderManager>();

//Email Settings
builder.Services.AddScoped<IEmailSender, EmailSender>(x=> 
new EmailSender("smtp.office365.com", 587, true,"mkavusdu.test@hotmail.com","deneme.123")
);



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
    MyInitialData.Seed();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();    // 3-1: 
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "orders",
    pattern: "/orders",
    defaults: new { controller = "Order", action = "Index" }
    );

//Cart / Sepetim iþlemleri
app.MapControllerRoute(
    name: "cart",
    pattern: "/cart",
    defaults: new { controller = "Cart", action = "Index" }
    );
//Cart / Sepetim iþlemleri
app.MapControllerRoute(
    name: "completeshopping",
    pattern: "/completeshopping",
    defaults: new { controller = "Cart", action = "CompleteShopping" }
    );
//User için route tanýmlarý
app.MapControllerRoute(
    name: "adminuserlist",
    pattern: "/admin/user/list",
    defaults: new { controller = "Admin", action = "UserList" }
    );
app.MapControllerRoute(
    name: "adminusercreate",
    pattern: "/admin/user/create",
    defaults: new { controller = "Admin", action = "UserCreate" }
    );
app.MapControllerRoute(
    name: "adminuseredit",
    pattern: "/admin/user/edit/{id?}",
    defaults: new { controller = "Admin", action = "UserEdit" }
    );

//Role için route tanýmlarý
app.MapControllerRoute(
    name: "adminrolelist",
    pattern: "/admin/role/list",
    defaults: new { controller = "Admin", action = "RoleList" }
    );
app.MapControllerRoute(
    name: "adminrolecreate",
    pattern: "/admin/role/create",
    defaults: new { controller = "Admin", action = "RoleCreate" }
    );
app.MapControllerRoute(
    name: "adminroleedit",
    pattern: "/admin/role/edit/{id?}",
    defaults: new { controller = "Admin", action = "RoleEdit" }
    );

// register/login/logut route tanýmlarý
app.MapControllerRoute(
    name: "forgotpassword",
    pattern: "forgotpassword",
    defaults: new { controller = "Account", action = "forgotpassword" }
    );

app.MapControllerRoute(
    name: "register",
    pattern: "register",
    defaults: new { controller = "Account", action = "register" }
    );

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "login" }
    );

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Account", action = "Logout" }
    );

//admin products create
app.MapControllerRoute(
    name: "admincategorycreate",
    pattern: "admin/categories/create",
    defaults: new { controller = "Admin", action = "CreateCategory" }
    );
//admin products edit
app.MapControllerRoute(
    name: "admincategoryedit",
    pattern: "admin/categories/{id}",
    defaults: new { controller = "Admin", action = "EditCategory" }
    );
//admin/categories
app.MapControllerRoute(
    name: "admincategorylist",
    pattern: "admin/categories",
    defaults: new { controller = "Admin", action = "CategoryList" }
    );

//admin products create
app.MapControllerRoute(
    name: "adminproductcreate",
    pattern: "admin/products/create",
    defaults: new { controller = "Admin", action = "CreateProduct" }
    );

//admin products edit
app.MapControllerRoute(
    name: "adminproductedit",
    pattern: "admin/products/{id}",
    defaults: new { controller = "Admin", action = "EditProduct" }
    );


//admin/products
app.MapControllerRoute(
    name: "adminproductlist",
    pattern: "admin/products",
    defaults: new { controller = "Admin", action = "ProductList" }
    );

//search
app.MapControllerRoute(
    name: "search",
    pattern: "search",
    defaults: new { controller = "product", action = "search" }
    );
//domain/about
app.MapControllerRoute(
    name: "about",
    pattern: "about",
    defaults: new { controller = "home", action = "about" }
    );
//domain/contact
app.MapControllerRoute(
    name: "contact",
    pattern: "contact",
    defaults: new { controller = "home", action = "contact" }
    );

//domain/products/{bilgisayar}
app.MapControllerRoute(
    name: "products",           // route'un adý.. eþsiz olacak
    pattern: "products/{category?}",    // adres çubuðunda domain'den sonra yazýlanlar ile eþleþecek olan isim/link/adres...
                                        // aders çubuðunda domain/products/{category}, category ilgili action'da yani list action'ýnýn parametresinde yakalanacak..
    defaults: new { controller = "Product", action = "List" }       //adres çubuðunda yazýlan adrese karþýlýk gelen pattern için çalýþacak olan controller ve action..
    );
// domain/iphone11
app.MapControllerRoute(
    name: "productdetails",
    pattern: "{url}",
    defaults: new { controller = "Product", action = "Details" }
    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");     //product/list

app.Run();
