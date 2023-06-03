using System.ComponentModel.DataAnnotations;

namespace CooksCornerAPP.ViewModels
{
    public class EditProfileViewModel
    {

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
