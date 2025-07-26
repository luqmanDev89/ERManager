using ERManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Admin,User")]
public class BaseController : Controller
{
    protected readonly UserManager<User> _userManager;

    public BaseController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    protected async Task<User> GetCurrentUserAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return null; // Or throw an exception if preferred
        }

        return await _userManager.FindByIdAsync(userId);
    }

    protected async Task<string> GetUserRoleAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault() ?? "User"; // Default to "User" if no roles are assigned
    }
}
