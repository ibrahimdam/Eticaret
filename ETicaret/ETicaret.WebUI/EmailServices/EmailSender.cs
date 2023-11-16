using System.Net;
using System.Net.Mail;

namespace ETicaret.WebUI.EmailServices
{
    public class EmailSender : IEmailSender
    {
        // Bu değişkenler Mail göndermek için SMTP Server'ın ihtiyacı olan değişkenlerdir..(SMTP server= Mail gönderen server)
        private string _host;
        private string _username;
        private string _password;
        private int _port;
        private bool _enableSSL;

        // Ben maili nereden göndereceğim ile ilgili parametreler...Yani benim SMTP server ve mail hesabım ile ilgili bilgiler constructor'a parametre olarak verilmiştir.. Bu bilgileri de Program.cs'de IoC altında verilmiştir.
        public EmailSender(string host, int port, bool enableSSL, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
            _enableSSL = enableSSL;
            _port = port;

        }

        //email: kime gidecek TO
        // subject: konu
        // htmlMessage (body): mesaj içeriği
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                //UseDefaultCredentials = false,
                Credentials =  new NetworkCredential(_username,_password),
                EnableSsl = _enableSSL
            };
            return client.SendMailAsync(
                new MailMessage(_username,email,subject, htmlMessage)
                {
                    IsBodyHtml = true
                });
        }
    }
}
