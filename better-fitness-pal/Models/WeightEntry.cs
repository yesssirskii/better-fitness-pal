namespace better_fitness_pal.Models;

public class WeightEntry
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly EntryDate { get; set; }
    public decimal WeightKg { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}