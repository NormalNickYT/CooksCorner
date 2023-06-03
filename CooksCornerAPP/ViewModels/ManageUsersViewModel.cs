using CooksCornerAPP.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CooksCornerAPP.ViewModels
{
    public class ManageUsersViewModel
    {

        
        public ApplicationUser User { get; set; }
        public string Role { get;set; }
        public List<SelectListItem> Roles { get; set; }


    }
}
