using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebApp;

[Route("api")]
public class LoginController : Controller
{
    private readonly IAccountDatabase _db;

    public LoginController(IAccountDatabase db)
    {
        _db = db;
    }

    [HttpPost("sign-in/{userName}")]
    public async Task<IActionResult> Login(string userName)
    {
        var account = await _db.FindByUserNameAsync(userName);
            
        if (account == null) return NotFound();
        //TODO_ 1: Generate auth cookie for user 'userName' with external id
        var claims = new List<Claim> {
            new (ClaimTypes.NameIdentifier, account.ExternalId),
            new (ClaimTypes.Role, account.Role),
            new (ClaimTypes.Name, account.UserName)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimsPrincipal);
        return Ok();
        //TODO_ 2: return 404 if user not found
    }

    [HttpGet("denied")]
    public ActionResult AccessDenied() => Unauthorized();
}