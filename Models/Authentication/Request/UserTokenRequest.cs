
using System.ComponentModel.DataAnnotations;

namespace Models.Authentication;

public class UserTokenRequest{

    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

}

