using Microsoft.AspNetCore.Identity;

namespace ETicaret.WebUI.Identity
{
    public class User : IdentityUser
    {
        // 1- Öncelikle Identity Frameworkünü yüklüyoruz. Bu Framework bize hazır bir takım alanları getiriyor fakat bunların dışında kullanmak istediğimiz property'ler var ise bir class oluşturup bu class üzerinde istediğimiz alanları oluşturabiliriz. Örneğin doğum tarihi cinsiyet bilgileri gibi...

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }

    }
}
