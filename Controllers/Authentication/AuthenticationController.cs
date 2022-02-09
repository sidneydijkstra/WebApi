using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Controllers.Authentication;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase{
    private readonly IAuthenticationService _authManager;

    public AuthenticationController(IAuthenticationService authManager){
        this._authManager = authManager;
    }

    [HttpPost]
    [Route("Register")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE_ADMIN")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userRequest){
        // Check if the incoming request is valid
        if (ModelState.IsValid){
            UserAuthenticationResponse authResult = await _authManager.regist(userRequest);
            return authResult.result ? Ok(authResult) : BadRequest(authResult);
        }
        
        return BadRequest(new UserAuthenticationResponse(){
            result = false,
            errors = new List<string>(){ "Invalid payload" }
        });
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest userRequest){
        // Check if the incoming request is valid
        if (ModelState.IsValid){
            UserAuthenticationResponse authResult = await _authManager.login(userRequest);
            return authResult.result ? Ok(authResult) : Ok(authResult);
        }
        
        return BadRequest(new UserAuthenticationResponse(){
            result = false,
            errors = new List<string>(){ "Invalid payload" }
        });
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] UserTokenRequest tokenRequest){
        // Check if the incoming request is valid
        if (ModelState.IsValid){
            UserAuthenticationResponse authResult = await _authManager.refresh(tokenRequest);
            return authResult.result ? Ok(authResult) : BadRequest(authResult);
        }
        
        return BadRequest(new UserAuthenticationResponse(){
            result = false,
            errors = new List<string>(){ "Invalid payload" }
        });
    }

}

