namespace better_fitness_pal.Models;

public class SavedMealItem
{
    public long Id { get; set; }
    public long SavedMealId { get; set; }
    public long FoodId { get; set; }
    public decimal Grams { get; set; }
    public int SortOrder { get; set; }

    public Food Food { get; set; } = null!;
}