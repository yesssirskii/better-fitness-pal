using better_fitness_pal.Auth;
using better_fitness_pal.Data;
using better_fitness_pal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace better_fitness_pal.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<AppUser> userManager,
    BetterFitnessPalDbContext db,
    TokenService tokens,
    TimeProvider time) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth!.Value
        };

        var created = await userManager.CreateAsync(user, request.Password);
        if (!created.Succeeded)
            return IdentityErrors(created);

        var roleAdded = await userManager.AddToRoleAsync(user, AppUserRoles.User);
        if (!roleAdded.Succeeded)
            return IdentityErrors(roleAdded);

        db.UserSettings.Add(new UserSettings { UserId = user.Id });

        var (response, _) = await IssueTokensAsync(user);
        await transaction.CommitAsync();

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return InvalidCredentials();

        if (await userManager.IsLockedOutAsync(user))
            return Problem(
                statusCode: StatusCodes.Status429TooManyRequests,
                title: "Too many failed attempts. Try again in a few minutes.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return InvalidCredentials();
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var (response, _) = await IssueTokensAsync(user);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
    {
        var now = time.GetUtcNow().UtcDateTime;
        var hash = TokenService.HashToken(request.RefreshToken);

        var stored = await db.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.TokenHash == hash);

        if (stored is null || stored.ExpiresAt <= now)
            return InvalidRefreshToken();

        await using var transaction = await db.Database.BeginTransactionAsync();

        var revoked = await db.RefreshTokens
            .Where(t => t.Id == stored.Id && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));

        if (revoked == 0)
        {
            // This token was already used once. Treat it as stolen and end every session for this user.
            await db.RefreshTokens
                .Where(t => t.UserId == stored.UserId && t.RevokedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));

            await transaction.CommitAsync();
            return InvalidRefreshToken();
        }

        var user = await userManager.FindByIdAsync(stored.UserId.ToString());
        if (user is null)
            return InvalidRefreshToken();

        var (response, newToken) = await IssueTokensAsync(user);

        await db.RefreshTokens
            .Where(t => t.Id == stored.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.ReplacedByTokenId, newToken.Id));

        await transaction.CommitAsync();
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request)
    {
        var now = time.GetUtcNow().UtcDateTime;
        var hash = TokenService.HashToken(request.RefreshToken);

        await db.RefreshTokens
            .Where(t => t.TokenHash == hash && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));

        return NoContent();
    }

    private async Task<(AuthResponse Response, RefreshToken Entity)> IssueTokensAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = tokens.CreateAccessToken(user, roles);
        var (refreshToken, refreshHash, refreshExpiresAt) = tokens.CreateRefreshToken();

        var entity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = time.GetUtcNow().UtcDateTime,
            ExpiresAt = refreshExpiresAt
        };
        db.RefreshTokens.Add(entity);
        await db.SaveChangesAsync();

        return (new AuthResponse(accessToken, accessExpiresAt, refreshToken, refreshExpiresAt), entity);
    }

    private ActionResult InvalidCredentials() =>
        Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid email or password.");

    private ActionResult InvalidRefreshToken() =>
        Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid or expired refresh token.");

    private ActionResult IdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(error.Code, error.Description);

        return ValidationProblem(ModelState);
    }
}