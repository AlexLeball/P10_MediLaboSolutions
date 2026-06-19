using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Frontend.Web.Helpers
{
    public static class JwtSessionHelper
    {
        public static IEnumerable<Claim> GetClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return Enumerable.Empty<Claim>();

            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims;
        }

        public static string? GetRole(string token)
            => GetClaims(token)
               .FirstOrDefault(c => c.Type == ClaimTypes.Role
                                 || c.Type == "role")
               ?.Value;

        public static bool IsAdmin(string token)
            => GetRole(token) == "Admin";
    }
}