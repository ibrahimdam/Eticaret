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

//3- Identity tan�mlamalar�:
builder.Services.AddDbContext<ApplicationContext>(option => option.UseSqlServer("Server=DESKTOP-MCJ9JKH;Database=ETicaret;Integrated Security=true"));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();


//4- Veritaban� tablolar�n� olu�turduktan sonra Identity ile ilgili bir tak�m �zelliklerin konfig�rasyonunu a�a��daki gibi yapabiliriz.
builder.Services.Configure<IdentityOptions>(options =>
{
    // password
    options.Password.RequireDigit = true;   // password^de mutlaka say�sal bir de�er olmal�.
    options.Password.RequireLowercase = true; //password'de mutlaka k���k harf olmal�.
    options.Password.RequireUppercase = true;   // password'de mutlaka b�y�k harf olmal�.
    options.Password.RequiredLength = 6;    // password en az 6 karakter olmal�.
    options.Password.RequireNonAlphanumeric = true; // rakam ve harf d���nda farkl� bir karakterin de password i�inde olmas� gerekiyor. �rn: nokta gibi, @ gibi, %,-,_ gibi karakterler...

    // lockout : Kullan�c� hesab�n�n klilitlenip kilitlenmemsi ile ilgili.
    options.Lockout.MaxFailedAccessAttempts = 5;    // yanl�� parolay� 5 kere girilebilir. Sonra hesap kilitlenir.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Hesap kilitlendikten 5 dakika sonra kullan�c� giri� yapmay� deneyebilir.

    //User
    options.User.RequireUniqueEmail=true;   // her kullan�c� tek bir email adresi ile sisteme girebilir. Yani uniq email kullan�l�r. Ayn� email ile 2 hesap a��lamaz.
    options.SignIn.RequireConfirmedEmail = false; // true olursa kullan�c� �ye olur fakat email'ini mutlaka onaylamas� gerekir. false olursa �ye olup hemen sisteme girebilir.
    options.SignIn.RequireConfirmedPhoneNumber = false; // True olursa telefon bilgisi i�in onay ister

});

// 4-1: Cookie ayarlar�: Cookie (�erez): Kullan�c�n�n taray�c�s�na b�rak�lan bir bilgi diyebiliriz k�saca. 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath= "/Account/Login";   // sisteme login olmad�ysak bizi login sayfas�na y�nlendiriyor. login olduysak da bize e�siz bir say� �retiyor.. Bu say�y� server taraf�nda session'da tutuluyor, Cilent taraf�nda ise cookie i�inde tutluyor.. Kullan�c� bir i�lem yapt�ktan sonra belirli bir s�re sonunda bu bilgi siliniyor. Belirtilen �zelliklere g�re, bu veri belirtilen s�re i�inde tekrar bir i�lem yap�l�rsa, tekrar login olmam�za gerek kalm�yor. Fakat s�re bittikten sonra tekrar login olmam�z gerekiyor.
    options.LogoutPath= "/Account/Logout";  //��k�� i�lemi yapt���mda cookie taray�c�dan silinecek Ve tekrar bir i�lem yapmak istedi�imde login sayfas�na y�nlendirilece�im.
    options.AccessDeniedPath= "/Account/Accessdenied"; // Yetkisiz i�lem yap�ld���nda �al��acak olan Action.. �rne�in s�radan bir kullan�c� Admin ile ilgili bir sayfaya ula�maya �al��t���nda �al��acak.

    options.SlidingExpiration = true; // �rne�in sisteme girdim i�lem yapt�m ve bekledim. varsay�lan de�er 20dakika. 20dakikadan sonra cookie'de bu bilgi silenecek. E�er 20 dakika i�inde tekrardan bir i�lem yaparsam bu s�re tekrardan 20 dakika olarak ayarlanacak. False olursa login olduktan sonra 20 dakika sonunda cookie silinecektir.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(300); // default s�resi 20dakika..

    options.Cookie = new CookieBuilder
    { HttpOnly = true, Name=".ETicaret.Security.Cookie"};
    // HttpOnly = true sadece http ile istek geldi�inde ula��labilir olsun diyoruz.
    // Name propertry'si ile de Cookie'ye �zel bir isim verebiliyoruz.
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

//Cart / Sepetim i�lemleri
app.MapControllerRoute(
    name: "cart",
    pattern: "/cart",
    defaults: new { controller = "Cart", action = "Index" }
    );
//Cart / Sepetim i�lemleri
app.MapControllerRoute(
    name: "completeshopping",
    pattern: "/completeshopping",
    defaults: new { controller = "Cart", action = "CompleteShopping" }
    );
//User i�in route tan�mlar�
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

//Role i�in route tan�mlar�
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

// register/login/logut route tan�mlar�
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
    name: "products",           // route'un ad�.. e�siz olacak
    pattern: "products/{category?}",    // adres �ubu�unda domain'den sonra yaz�lanlar ile e�le�ecek olan isim/link/adres...
                                        // aders �ubu�unda domain/products/{category}, category ilgili action'da yani list action'�n�n parametresinde yakalanacak..
    defaults: new { controller = "Product", action = "List" }       //adres �ubu�unda yaz�lan adrese kar��l�k gelen pattern i�in �al��acak olan controller ve action..
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
