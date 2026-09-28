using System.ComponentModel.DataAnnotations;

namespace better_fitness_pal.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required] public string Issuer { get; init; } = "";
    [Required] public string Audience { get; init; } = "";
    [Required, MinLength(64)] public string SigningKey { get; init; } = "";
    [Range(1, 60)] public int AccessTokenMinutes { get; init; } = 15;
    [Range(1, 90)] public int RefreshTokenDays { get; init; } = 30;
}