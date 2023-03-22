using System.Security.Claims;

namespace WebApi.Auth {
    public interface IJwtService {
        string GenerateSecurityToken(string email, IEnumerable<string> role, IEnumerable<Claim> userClaims);
    }
}
