
using System.Collections.Generic;

namespace Models.Authentication;
public class UserAuthenticationResponse
{
    public string token { get; set; } = string.Empty;
    public string refreshToken { get; set; } = string.Empty;
    public string roles { get; set; } = string.Empty;
    public bool result { get; set; }
    public List<string>? errors { get; set; }
}

