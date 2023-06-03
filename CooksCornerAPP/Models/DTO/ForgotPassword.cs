using System.ComponentModel.DataAnnotations;

namespace CooksCornerAPP.Models.DTO
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
