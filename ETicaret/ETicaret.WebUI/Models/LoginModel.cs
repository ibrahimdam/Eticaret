//using Microsoft.Build.Framework; //bakılacak..
using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name ="Kullanıcı Adınız")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Şifreniz")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public string ReturnUrl { get; set; }

    }
}
