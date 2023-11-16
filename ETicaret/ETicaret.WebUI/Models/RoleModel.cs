using ETicaret.WebUI.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ETicaret.WebUI.Models
{
    public class RoleModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class RoleEditModel
    {
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[] IdsToAddRole { get; set; }
        public string[] IdsToDeleteRole { get; set; }
    }

    public class RoleDetails
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
    }
}
