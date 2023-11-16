namespace ETicaret.WebUI.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }

        public IEnumerable<string> SelectedRoles { get; set; }
        
    }
}
