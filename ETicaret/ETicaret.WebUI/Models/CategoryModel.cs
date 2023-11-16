using ETicaret.Entities;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ETicaret.WebUI.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Display(Name = "Kategori Adı", Prompt = "Kategori Adını Giriniz")]
        [Required(ErrorMessage = "Kategori Adı zorunlu bir alandır. Boş geçilemez.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Kategori Adı en az 5, en fazla 100 karakter olmalıdır.")]
        public string Name { get; set; }
        [Display(Name = "Kategori Linki", Prompt = "Kategori Linki'ni Giriniz")]
        [Required(ErrorMessage = "Kategori Linki zorunlu bir alandır. Boş geçilemez.")]
        public string Url { get; set; }
        public List<Product> Products { get; set; }
    }
}
