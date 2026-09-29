using better_fitness_pal.Auth;
using better_fitness_pal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace better_fitness_pal.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController(UserManager<AppUser> userManager) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> Me()
    {
        var user = await userManager.FindByIdAsync(User.GetUserId().ToString());
        if (user is null)
            return NotFound();

        return new UserProfileResponse(
            user.Id, user.Email!, user.UserName!,
            user.FirstName, user.LastName, user.DateOfBirth, user.AvatarPath);
    }
}

public record UserProfileResponse(
    Guid Id,
    string Email,
    string UserName,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string? AvatarPath);