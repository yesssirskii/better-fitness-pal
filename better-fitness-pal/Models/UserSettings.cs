namespace better_fitness_pal.Models;

public class UserSettings
{
    public Guid UserId { get; set; }
    public string WeightUnit { get; set; } = "kg";
    public decimal? WeightGoalKg { get; set; }
    public decimal? WeeklyGoalKg { get; set; }
    public string? ActivityLevel { get; set; }
    public int? CalorieGoal { get; set; }
    public short? ProteinPct { get; set; }
    public short? CarbsPct { get; set; }
    public short? FatPct { get; set; }
    public DateTime? UpdatedAt { get; set; }
}