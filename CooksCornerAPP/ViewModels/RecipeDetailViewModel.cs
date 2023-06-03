namespace CooksCornerAPP.ViewModels
{
    public class RecipeDetailViewModel
    {

        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string OwnerUserName { get; set; }
        public byte[] Image { get; set; }
        public string Category { get; set; }
        public string Ingredients { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan AdditionalTime { get; set; }
        public TimeSpan CookTime { get; set; }
        public TimeSpan PrepTime { get; set; }
        public int Servings { get; set; }
        public string Description { get; set; }

    }
}
