using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Authentication;
public class AuthenticationUser : IdentityUser
{

    [NotMapped]
    public List<string> roles_ { get; set; } = new List<string>();

    [Required]
    public string roles{
        get { return String.Join(',', roles_); }
        set { roles_ = value.Split(',').ToList(); }
    }
}
