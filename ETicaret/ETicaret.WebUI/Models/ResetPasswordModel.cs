using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [Display(Name = "E-Postanız")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Şifreniz")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
