using ETicaret.Entities;
using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Display(Name="Ürün Adı", Prompt = "Ürün Adını Giriniz")]
        [Required(ErrorMessage ="Ürün Adı zorunlu bir alandır. Boş geçilemez.")]
        [StringLength(100, MinimumLength =5, ErrorMessage ="Ürün Adı en az 5, en fazla 100 karakter olmalıdır.")]
        public string Name { get; set; }

        [Display(Name = "Ürün Linki", Prompt = "Ürün Linki'ni Giriniz")]
        [Required(ErrorMessage = "Ürün Linki zorunlu bir alandır. Boş geçilemez.")]
        public string Url { get; set; }

        [Display(Name = "Ürün Fiyatı")]
        [Required(ErrorMessage = "Ürün Fiyatı zorunlu bir alandır. Boş geçilemez.")]
        [Range(1,1000000, ErrorMessage ="Ürün Fiyatı için 1 - 1.000.000 TL arasında bir değer giriniz.")]
        public double? Price { get; set; }

        [Display(Name = "Ürün Açıklması", Prompt = "Ürün Açıklması Giriniz")]
        [Required(ErrorMessage = "Ürün Açıklması zorunlu bir alandır. Boş geçilemez.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Ürün Açıklaması en az 5, en fazla 1000 karakter olmalıdır.")]
        public string Description { get; set; }

        [Display(Name = "Ürün Fotoğrafı", Prompt = "Ürün Fotoğrafı Giriniz")]
        [Required(ErrorMessage ="Ürün Fotoğrafı zorunlu bir alandır. Boş geçilemez.")]
        public string ImageUrl { get; set; }
        [Display(Name = "Onaylı mı?")]
        public bool IsApproved { get; set; }
        [Display(Name = "Anasayfa?")]
        public bool IsHome { get; set; }

        public List<Category> SelectedCategories { get; set; }

        public ProductModel()
        {
            SelectedCategories = new List<Category>();
        }
    }
}
