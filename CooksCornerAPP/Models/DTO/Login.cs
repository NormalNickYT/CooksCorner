using System.ComponentModel.DataAnnotations;

namespace CooksCornerAPP.Models.DTO
{
    public class Login
    {

        [Required]
        public string? Email { get; set; }


        [Required]
        public string? Password { get; set; }

        public string Token { get; set; }

    }
}
