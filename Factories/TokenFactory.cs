using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Factories;
public static class TokenFactory{

    private static JwtSecurityTokenHandler _jwtTokenHandler = new JwtSecurityTokenHandler();
    private static string STRING_GENERATION_SET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string WriteToken(SecurityToken token){
        return _jwtTokenHandler.WriteToken(token);
    }

    public static SecurityToken GenerateJwtToken(string secret, AuthenticationUser user, int lifetime) {
        var key = Encoding.ASCII.GetBytes(secret);

        // create identity
        var identity = new ClaimsIdentity(new[]{
            new Claim("Id", user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, GenerateUUID())
        });

        // add roles
        foreach (string roles in user.roles_) {
            identity.AddClaim(new Claim(ClaimTypes.Role, roles));
        }

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = identity,
            Expires = DateTime.UtcNow.AddSeconds(lifetime), // time token is valid
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),

        };

        return _jwtTokenHandler.CreateToken(tokenDescriptor);
    }

    public static RefreshToken GenerateRefreshToken(SecurityToken token, AuthenticationUser user, float expireTimeSeconds) { 
        return new RefreshToken() {
            JwtId = token.Id,
            IsUsed = false,
            UserId = user.Id,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.Now.AddSeconds(expireTimeSeconds),
            IsRevoked = false,
            Token = GenerateRandomString(25) + GenerateUUID()
        };
    }

    public static bool VerifyToken(ClaimsPrincipal principal, RefreshToken refreshToken) { 
        // Check the date of the saved token if it has expired
        if (DateTime.UtcNow > refreshToken.ExpiryDate){
            return false;
        }

        // check if the refresh token has been used
        if (refreshToken.IsUsed){
            return false;
        }

        // Check if the token is revoked
        if (refreshToken.IsRevoked){
            return false;
        }

        // we are getting here the jwt token id
        var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        // check the id that the recieved token has against the id saved in the db
        if (refreshToken.JwtId != jti){
            return false;
        }

        return true;
    }

    public static string GenerateUUID() {
        return Guid.NewGuid().ToString();
    }

    public static string GenerateRandomString(int length){
        var random = new Random();
        return new string(Enumerable.Repeat(STRING_GENERATION_SET, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp){
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dtDateTime;
    }
}
