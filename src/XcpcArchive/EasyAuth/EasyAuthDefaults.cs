using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using XcpcArchive.EasyAuth;

namespace Microsoft.AspNetCore.Builder
{
    public static class EasyAuthDefaults
    {
        public const string AuthenticationScheme = "EasyAuth";

        public static AuthenticationBuilder AddEasyAuth(
            this AuthenticationBuilder builder,
            Action<EasyAuthAuthenticationOptions>? configureOptions = null)
            => builder
                .AddScheme<EasyAuthAuthenticationOptions, EasyAuthAuthenticationHandler>(
                    AuthenticationScheme,
                    AuthenticationScheme,
                    configureOptions);

        public static IServiceCollection AddEasyAuthAuthentication(
            this IServiceCollection services,
            Action<EasyAuthAuthenticationOptions>? configureOptions = null)
            => services
                .AddAuthentication(AuthenticationScheme)
                .AddEasyAuth(configureOptions)
                .Services;
    }
}
