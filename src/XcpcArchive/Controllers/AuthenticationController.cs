using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using XcpcArchive.EasyAuth;

namespace XcpcArchive.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        [HttpGet("/api/user")]
        public ActionResult<EasyAuthClientPrincipal> GetClaims()
        {
            return new EasyAuthClientPrincipal
            {
                NameType = ((ClaimsIdentity?)User.Identity)?.NameClaimType!,
                RoleType = ((ClaimsIdentity?)User.Identity)?.RoleClaimType!,
                AuthenticationType = User.Identity?.AuthenticationType!,
                Claims = User.Claims.Select(c => new EasyAuthClientPrincipal.UserClaim { Type = c.Type, Value = c.Value }),
            };
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return Redirect("/.auth/login/aad?post_login_redirect_uri=%2F");
        }

        [HttpGet("/logout")]
        public IActionResult Logout()
        {
            return Redirect("/.auth/logout?post_logout_redirect_uri=%2F");
        }
    }
}
