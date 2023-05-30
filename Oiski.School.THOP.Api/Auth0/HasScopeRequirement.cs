using Microsoft.AspNetCore.Authorization;

namespace Oiski.School.THOP.Api.Auth0
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public HasScopeRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer)); ;
        }

        public string Issuer { get; set; }
        public string Scope { get; set; }
    }
}
