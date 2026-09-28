namespace better_fitness_pal.Models;

public class Food
{
    public long Id { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public required string Name { get; set; }
    public string? Brand { get; set; }
    public string? Barcode { get; set; }
    public decimal? ServingSizeG { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}