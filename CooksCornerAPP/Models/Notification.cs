using CooksCornerAPP.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CooksCornerAPP.Models
{
    public class Notification
    {

        public int Id { get; set; }

        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Message { get; set; }

        [ForeignKey("RecipeId")]
        public int? RecipeId { get; set; }
        public virtual Recipes Recipe { get; set; }

        public bool IsRead { get; set; }

    }

}
