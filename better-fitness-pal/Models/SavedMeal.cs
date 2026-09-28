namespace better_fitness_pal.Models;

public class SavedMeal
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<SavedMealItem> Items { get; set; } = [];
}