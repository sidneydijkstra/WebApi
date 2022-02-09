using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Authentication;
public class SecureUser {

    public string id {get; set;} = string.Empty;
    public string username {get; set;} = string.Empty;
    public string email {get; set;} = string.Empty;
    public string roles {get; set;} = string.Empty;
    public bool disabled {get; set;}
    public DateTime createDate {get; set;}
}
