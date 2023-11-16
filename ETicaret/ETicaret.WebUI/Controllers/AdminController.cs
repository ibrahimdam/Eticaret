using ETicaret.BusinessLayer.Abstract;
using ETicaret.Entities;
using ETicaret.WebUI.Identity;
using ETicaret.WebUI.Models;
using ETicaret.WebUI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.WebUI.Controllers
{
    // ilgili tanımlamaları Program.cs dosyasından yaptıktan sonra bir controller'a [Authorize] attribute'unu verdiğimde, artık bu Controller'a sadece sisteme login olan ulaşabilecek.. Bunun için Program.cs içinde app.UseAuthorization(); eklenm,iş olması gerekiyor.
    //linke baktığımızda https://localhost:7024/Account/Login?ReturnUrl=%2Fadmin%2Fproducts sisteme login olmadığımız için bizi login sayfasına yönlendiriyor. login başarılı olursa ReturnUrl parametresi ile gidilecek sayfaya yönlnedirileceğiz.
    [Authorize(Roles="Admin")]  // Authorize ile kullanıcının sisteme login olması gerekirken, login olan kullanıcının Admin rolüne de sahip olması gerekiyor AdminController altındaki Actionlara ulaşabilmesi için..
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminController(IProductService productService, ICategoryService categoryService, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // Role işlemleri
        // ahmet => admin 
        // ali => customer
        // veli => seller
        //[AllowAnonymous]
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }
        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                // valid=true ise kayıt işlemi yapılacak.

               var result = await _roleManager.CreateAsync(new IdentityRole() { Name=model.Name});
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");

                } 
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }


            }
            return View(model);
            
        }
        [HttpGet]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users.ToList())
            {
               var list = await _userManager.IsInRoleAsync(user, role.Name) ? members:nonmembers;
                list.Add(user);
                //Farklı bir yol.. yukarıdaki 2 satyır ile aynı işi yapıyor.
                //if (await _userManager.IsInRoleAsync(user, role.Name))
                //{
                //    members.Add(user);
                //}
                //else
                //{
                //    nonmembers.Add(user);
                //}
            }

            var model = new RoleDetails()
            {
                Members = members,
                NonMembers = nonmembers,
                Role =role
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            ModelState.Remove("IdsToDeleteRole");
            ModelState.Remove("IdsToAddRole");
            if (ModelState.IsValid)
            {
                // aşağıdaki foreach ile yeni kullanıcılara ilgili rol'ü atamak için kulandık
                foreach (var userId in model.IdsToAddRole ?? new string[] { } )
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user!= null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("",item.Description);
                            }
                        }
                    }
                }
                // Aşağıdaki foreach ile rolleri ilgili kullanıcılardan siliyoruz.

                foreach (var userId in model.IdsToDeleteRole ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("", item.Description);
                            }
                        }
                    }
                }
            }


            return Redirect("/admin/role/edit/" + model.RoleId);
        }

        // User/Kullanıcı işlemleri
        // Listeleme işlemini 
        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }
        [HttpGet]
        public IActionResult UserCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserCreate(UserModel model)
        {
            if (ModelState.IsValid)
            {
                

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(string id)
        {
            // ilgili user'ı bulacağız ve bunu View'e göndereceğiz..
            // ilgili view'de düzenlememizi yapıp UserEdit HttpPost olan actiona göndereceğiz.
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Seçilen kullanıcı için atanmış bütün rolleri veritabanından alıyoruz.
                var selectedRoles =await _userManager.GetRolesAsync(user);

                var roles = _roleManager.Roles.Select(x=> x.Name);
                ViewBag.Roles = roles;

                var userModel = new UserModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                };
                return View(userModel);
            }


            return Redirect("/admin/user/list");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserModel model, string[] selectedRoles)
        {
            // Gelen güncellenmiş User verisini veritabanından update edeceğiz.
            ModelState.Remove("Password");
            ModelState.Remove("RePassword");
            //ModelState.Remove("SelectedRoles");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.EmailConfirmed = model.EmailConfirmed;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);     // ilgili user'a ait roller veritabanından getiriliyor.
                        selectedRoles = selectedRoles ?? new string[] { };

                        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                        // yukarıda Except ile selectRoles içinde kullanıcının sahip olduğu roller çıkarılıyor ve kalan roller kullanıcı için veritabanına ekleniyor.. Kısacası yeni eklenen roller bu kullanıcı için ilgili tabloya ekleniyor.

                        await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToArray<string>());

                        return Redirect("/admin/user/list");
                    }
                }

                return Redirect("/admin/user/list");
            }

            return View(model);
        }

        // Ürün işlemleri
        public IActionResult ProductList()
        {
            var productListViewModel = new ProductListViewModel()
            {
                Products = _productService.GetAll()
            };
            return View(productListViewModel);
        }

        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();  //404: not Found
            }
            Console.WriteLine(id);
            Product entity = _productService.GetByIdWithCategories(id.Value);
            if (entity == null)
            {
                return NotFound();
            }
            ProductModel model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Url = entity.Url,
                ImageUrl = entity.ImageUrl,
                Price = entity.Price,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,

                SelectedCategories = entity.ProductCategories.Select(x => x.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();
            //ViewBag ile veritabanındaki bütün kategorileri alıp View'e gönderiyoruz ki product için hangi kategoriler eklenecekse onları seçebilelim.
            return View(model);
        }

        [HttpPost]
        public IActionResult EditProduct(ProductModel model, int[] categoryIds, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                Product product = _productService.GetById(model.Id);
                if (product == null)
                {
                    return NotFound();
                }
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Url = model.Url;
                //product.ImageUrl = model.ImageUrl;
                product.IsHome = model.IsHome;
                product.IsApproved = model.IsApproved;

                if (file!= null)
                {
                    // Gelen dosyanın uzantısını alıyorum.
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var imageName = string.Format($"{Guid.NewGuid()}{extension}");
                    product.ImageUrl = imageName;
                    //Dosyanın nereye kaydedileceğini aşağıdaki satırda veriyorum.
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);

                    // Aşağıdaki satırda da dosya yolu ve dosya adı verilen yere dosyayı kaydediyoruz.
                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }


                _productService.Update(product, categoryIds);
                return RedirectToAction("ProductList");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductModel model, IFormFile? file)
        {
            // Validation işlemini yap
            ModelState.Remove("ImageUrl");
            if (ModelState.IsValid)
            {
                Product product = new Product()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Url = model.Url,
                    //ImageUrl = model.ImageUrl,
                    IsApproved = model.IsApproved,
                    IsHome = model.IsHome
                };
                if (file != null)
                {
                    // Gelen dosyanın uzantısını alıyorum.
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var imageName = string.Format($"{Guid.NewGuid()}{extension}");
                    product.ImageUrl = imageName;
                    //Dosyanın nereye kaydedileceğini aşağıdaki satırda veriyorum.
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);

                    // Aşağıdaki satırda da dosya yolu ve dosya adı verilen yere dosyayı kaydediyoruz.
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                else
                {
                    product.ImageUrl = "no-product-image.jpg";
                }
                _productService.Create(product);
                return RedirectToAction("ProductList");
            }

            return View(model);

        }

        public IActionResult DeleteProduct(int? id)
        {
            if (id == null)
                return NotFound();
            Product product = _productService.GetById(id.Value);
            if (product == null)
                return NotFound();
            _productService.Delete(product);
            return RedirectToAction("ProductList");
        }

        //***************************************************
        public IActionResult CategoryList()
        {
            var categoryListViewModel = new CategoryListViewModel()
            {
                Categories = _categoryService.GetAll()
            };
            return View(categoryListViewModel);
        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();  //404: not Found
            }

            //Category entity = _categoryService.GetById(id.Value);
            Category entity = _categoryService.GetByIdWithProducts(id.Value);
            if (entity == null)
            {
                return NotFound();
            }
            CategoryModel model = new CategoryModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(x => x.Product).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            ModelState.Remove("Products");
            if (ModelState.IsValid)
            {
                Category category = _categoryService.GetById(model.Id);
                if (category == null)
                {
                    return NotFound();
                }
                category.Name = model.Name;
                category.Url = model.Url;


                _categoryService.Update(category);
                return RedirectToAction("CategoryList");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            ModelState.Remove("Products");
            if (ModelState.IsValid)
            {
                Category category = new Category()
                {
                    Name = model.Name,
                    Url = model.Url
                };


                _categoryService.Create(category);
                return RedirectToAction("CategoryList");
            }
            return View(model);
        }

        public IActionResult DeleteCategory(int? id)
        {
            if (id == null)
                return NotFound();
            Category category = _categoryService.GetById(id.Value);
            if (category == null)
                return NotFound();
            _categoryService.Delete(category);
            return RedirectToAction("CategoryList");
        }
        [HttpPost]
        public IActionResult DeleteProductFromCategory(int productId, int categoryId)
        {
            // Ürünü silmeyeceğiz. Kategori tarafında görüntülenen Ürünün Bu category ile olan bağlantısını sileceğiz.
            _categoryService.DeleteProductFromCategory(productId, categoryId);
            return Redirect("/admin/categories/" + categoryId);
        }
    }
}
