using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CooksCornerAPP.ViewModels
{
    public class EditUserViewModel
    {

        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
