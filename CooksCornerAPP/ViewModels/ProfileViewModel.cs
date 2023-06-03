using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CooksCornerAPP.ViewModels
{
    public class ProfileViewModel
    {

        public string Id { get; set; }


        public string Name { get; set; }

        public string Email { get; set; }

        public byte Image { get; set; }
    }
}
