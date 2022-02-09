using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Controllers.Authentication;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase{
    private readonly IAuthenticationService _authManager;

    public AdminController(IAuthenticationService authManager){
        this._authManager = authManager;
    }

    [HttpGet]
    [Route("ListUsers")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE_ADMIN")]
    public  IActionResult ListUsers(){
        // Check if the incoming request is valid
        UserListResponse response = _authManager.getUsers();
        return Ok(response);
    }

}

