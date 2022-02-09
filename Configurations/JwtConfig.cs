
namespace Configurations;

public class JwtConfig
{
    public string Secret { get; set; } = string.Empty;
    public int JwtTokenLifetime { get; set; }
    public int RefreshTokenLifetime { get; set; }
}
