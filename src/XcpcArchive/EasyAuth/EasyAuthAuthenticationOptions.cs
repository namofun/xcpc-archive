using Microsoft.AspNetCore.Authentication;

namespace XcpcArchive.EasyAuth
{
    public class EasyAuthAuthenticationOptions : AuthenticationSchemeOptions
    {
        public bool DeveloperMode { get; set; }

        public EasyAuthAuthenticationOptions()
        {
            Events = new object();
        }
    }
}
