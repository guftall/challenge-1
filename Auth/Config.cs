using System.Collections.Generic;
using IdentityServer4.Models;

namespace Auth
{
    public static class Config
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "user.manage", displayName: "Manage users"),
                new ApiScope(name: "ads", displayName: "Ads")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "adsserver",

                    AllowedGrantTypes = new List<string>
                    {
                        GrantType.ResourceOwnerPassword
                    },

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AccessTokenLifetime = 7 * 24 * 60 * 60,
                    AllowedScopes = {"ads", "user.manage"}
                }
            };
        }
    }
}