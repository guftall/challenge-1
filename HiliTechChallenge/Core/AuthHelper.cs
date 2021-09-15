using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace HiliTechChallenge.Core
{
    public class AuthHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task AddUser(string token, string email, string password)
        {
            var client = _httpClientFactory.CreateClient("auth");

            var jsonString = JsonConvert.SerializeObject(new
            {
                email,
                password
            });
            client.SetBearerToken(token);
            var request = new HttpRequestMessage(HttpMethod.Post, "/auth/user")
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };
            var httpResponse = await client.SendAsync(request);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var responseStr = await httpResponse.Content.ReadAsStringAsync();
                throw new BadRequestException(responseStr);
            }
        }

        public async Task<string> GetAccessToken(string username, string password, bool isAdmin)
        {
            var client = _httpClientFactory.CreateClient("auth");

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://hili.guftall.ir/auth",
                Policy =
                {
                    RequireHttps = false
                }
            });

            var scope = isAdmin ? "ads user.manage" : "ads";
            var tokenResult = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "adsserver",
                ClientSecret = "secret",
                UserName = username,
                Password = password,
                Scope = scope
            });

            if (tokenResult.IsError)
            {
                return null;
            }


            var role = Utilities.JwtTokenClaims(tokenResult.AccessToken).First(c => c.Type.Equals("role"));
            if (isAdmin && role.Value != "admin")
            {
                return null;
            }

            return tokenResult.AccessToken;
        }
    }
}