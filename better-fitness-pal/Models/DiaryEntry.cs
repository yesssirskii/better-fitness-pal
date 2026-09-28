namespace better_fitness_pal.Models;

public class DiaryEntry
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly EntryDate { get; set; }
    public short MealSlot { get; set; }
    public long? FoodId { get; set; }
    public long? SavedMealId { get; set; }
    public decimal Grams { get; set; }
    public required string FoodName { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}