using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace XcpcArchive.EasyAuth
{
    public class EasyAuthAuthenticationHandler : AuthenticationHandler<EasyAuthAuthenticationOptions>
    {
        public EasyAuthAuthenticationHandler(
            IOptionsMonitor<EasyAuthAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        private EasyAuthClientPrincipal? GetClaimsPrincipal(out string? authenticationScheme)
        {
            string? enabledEnv = Environment.GetEnvironmentVariable("WEBSITE_AUTH_ENABLED", EnvironmentVariableTarget.Process);
            if (!string.Equals(enabledEnv, "True", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Options.DeveloperMode)
                {
                    authenticationScheme = "develop";
                    return new EasyAuthClientPrincipal
                    {
                        NameType = "name",
                        RoleType = "role",
                        AuthenticationType = authenticationScheme,
                        Claims = new[]
                        {
                            new EasyAuthClientPrincipal.UserClaim { Type = "name", Value = "Developer" },
                            new EasyAuthClientPrincipal.UserClaim { Type = "sub", Value = "Developer" },
                            new EasyAuthClientPrincipal.UserClaim { Type = "role", Value = "XcpcArchiveUploader" },
                        }
                    };
                }
                else
                {
                    authenticationScheme = null;
                    return null;
                }
            }

            authenticationScheme = Context.Request.Headers["X-MS-CLIENT-PRINCIPAL-IDP"].FirstOrDefault();
            string? msClientPrincipalEncoded = Context.Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(msClientPrincipalEncoded)
                || string.IsNullOrWhiteSpace(authenticationScheme))
            {
                return null;
            }

            byte[] decodedBytes = Convert.FromBase64String(msClientPrincipalEncoded);
            string msClientPrincipalDecoded = Encoding.Default.GetString(decodedBytes);
            return JsonConvert.DeserializeObject<EasyAuthClientPrincipal>(msClientPrincipalDecoded);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                EasyAuthClientPrincipal? clientPrincipal = GetClaimsPrincipal(out string? easyAuthProvider);
                if (clientPrincipal == null || easyAuthProvider == null)
                {
                    return Task.FromResult(AuthenticateResult.NoResult());
                }

                ClaimsPrincipal principal = new(
                    new ClaimsIdentity(
                        clientPrincipal.Claims.Select(x => new Claim(x.Type, x.Value)).ToList(),
                        clientPrincipal.AuthenticationType,
                        clientPrincipal.NameType,
                        clientPrincipal.RoleType));

                Context.User = principal;
                return Task.FromResult(
                    AuthenticateResult.Success(
                        new AuthenticationTicket(principal, easyAuthProvider)));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (Request.Path.StartsWithSegments("/api"))
            {
                Response.StatusCode = 401;
                return Task.CompletedTask;
            }
            else
            {
                Response.Redirect("/.auth/login/aad");
                return Task.CompletedTask;
            }
        }
    }
}
