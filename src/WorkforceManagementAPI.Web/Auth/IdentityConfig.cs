using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WorkforceManagementAPI.Web.Auth
{
    [ExcludeFromCodeCoverage]
    public class IdentityConfig
    {
        public static IEnumerable<Client> Clients =>
           new List<Client>
           {
                new Client
                {
                    ClientId = "workforcemanagementapi",
                    AllowOfflineAccess = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "users", "offline_access", "workforcemanagementapi", "roles" }
                }
           };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResource("roles", new[] { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                    new ApiScope("users", "My API", new string[]{ ClaimTypes.Name, ClaimTypes.Role }),
                    new ApiScope("offline_access", "RefereshToken"),
                    new ApiScope("workforcemanagementapi", "app")
            };
    }
}
