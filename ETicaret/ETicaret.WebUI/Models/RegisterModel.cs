using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name ="Adınız")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Soyadınız")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Kullanıcı Adınız")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "E-Postanız")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Şifreniz")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Şifre Tekrar")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string RePassword { get; set; }

    }
}
