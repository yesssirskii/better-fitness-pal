using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace better_fitness_pal.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? throw new InvalidOperationException("No 'sub' claim on the current user."));
}