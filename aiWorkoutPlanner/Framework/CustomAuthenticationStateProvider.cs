using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace aiWorkoutPlanner.Framework
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            var token = await SecureStorage.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token) || !token.Equals("0"))
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        
        public async Task MarkUserAsAuthenticated(string email, string jwtToken)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email)
            }, "apiauth_type"));

            await SecureStorage.SetAsync("auth_token", jwtToken);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
        }


        public async Task MarkUserAsLoggedOut()
        {
            await SecureStorage.SetAsync("auth_token", "0");

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out var nameIdentifier) && nameIdentifier != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier.ToString()));
            }
            else
            {
                // Handle the case where the claim is not found
            }

            return claims;
        }


        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
