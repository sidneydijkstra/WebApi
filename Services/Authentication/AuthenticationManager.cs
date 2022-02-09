using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Authentication;

/// <summary>
/// The AuthenticationManager class is an implementation of the interface IAuthenticationService.
/// This class can be used as Authentication service.
/// </summary>
public class AuthenticationManager : IAuthenticationService {

    private readonly UserManager<AuthenticationUser> _userManager;
    private readonly RolesConfig _rolesConfig;
    private readonly JwtConfig _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParams;
    private readonly TokenValidationParameters _refreshTokenValidationParams;
    private readonly APIDatabaseContext _database;

    /// <summary>
    /// The is the constructor of the AuthenticationManager.
    /// </summary>
    public AuthenticationManager(UserManager<AuthenticationUser> userManager, IOptionsMonitor<RolesConfig> optionsMonitorRoles, IOptionsMonitor<JwtConfig> optionsMonitorJwt, TokenValidationParameters tokenValidationParams, APIDatabaseContext database){
        this._userManager = userManager;
        this._rolesConfig = optionsMonitorRoles.CurrentValue;
        this._jwtConfig = optionsMonitorJwt.CurrentValue;
        this._tokenValidationParams = tokenValidationParams;
        this._database = database;

        // setup refresh token validation parameters, they are a bit diffrent.
        this._refreshTokenValidationParams = tokenValidationParams.Clone();
        this._refreshTokenValidationParams.ValidateLifetime = false;
    }

    /// <summary>
    /// This is a theaded method used to login a user.
    /// It wil login a user to the system.
    /// </summary>
    /// <param name="request">A request containing details to login a user.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    public async Task<UserAuthenticationResponse> login(UserLoginRequest request){
        AuthenticationUser user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null){
            return new UserAuthenticationResponse(){
                result = false,
                errors = new List<string>(){"Invalid authentication request"}
            };
        }

        bool passwordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (passwordCorrect){
            return await GenerateJwtToken(user);
        }else{
            return new UserAuthenticationResponse(){
                result = false,
                errors = new List<string>(){"Invalid authentication request"}
            };
        }
    }

    /// <summary>
    /// This is a theaded method used to regist a new user.
    /// It wil regist a new user to the system.
    /// </summary>
    /// <param name="request">A request containing details to regist a user.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    public async Task<UserAuthenticationResponse> regist(UserRegistrationRequest request){
        AuthenticationUser user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null){
            return new UserAuthenticationResponse(){
                result = false,
                errors = new List<string>(){"Invalid authentication request"}
            };
        }

        AuthenticationUser newUser = new AuthenticationUser() { Email = request.Email, UserName = request.Email, roles_ = new List<String>() { _rolesConfig.Default } };
        var userCreated = await _userManager.CreateAsync(newUser, request.Password);
        if (userCreated.Succeeded){
            return await GenerateJwtToken(newUser);
        }else {
            return new UserAuthenticationResponse(){
                result = false,
                errors = new List<string>() { "Invalid authentication request" }
            };
        }
    }

    /// <summary>
    /// This is a theaded method used to refresh the user.
    /// It wil use generate a new JwtToken for the user using a RefreshToken.
    /// </summary>
    /// <param name="request">A request containing details to generate a new Token.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    public async Task<UserAuthenticationResponse> refresh(UserTokenRequest request){
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        try{
            // This validation function will make sure that the token meets the validation parameters
            // and its an actual jwt token not just a random string
            var principal = jwtTokenHandler.ValidateToken(request.Token, this._refreshTokenValidationParams, out var validatedToken);

            // Now we need to check if the token has a valid security algorithm
            if (validatedToken is JwtSecurityToken jwtSecurityToken){
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                if (result == false){
                    return new UserAuthenticationResponse(){
                        result = false,
                        errors = new List<string>() { "Invalid authentication request" }
                    };
                }
            }

            // Will get the time stamp in unix time
            var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            // we convert the expiry date from seconds to the date
            var expDate = TokenFactory.UnixTimeStampToDateTime(utcExpiryDate);
            if (expDate > DateTime.UtcNow)
            {
                return new UserAuthenticationResponse(){
                    result = false,
                    errors = new List<string>() { "Invalid authentication request" }
                };
            }

            // Check the token we got if its saved in the db
            RefreshToken refreshToken = _database.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
            if (refreshToken == null){
                return new UserAuthenticationResponse(){
                    result = false,
                    errors = new List<string>() { "Invalid authentication request" }
                };
            }
            
            // check if refresh token and JWT token combination is correct
            bool tokenCorrect = TokenFactory.VerifyToken(principal, refreshToken); ;
            if (tokenCorrect) {
                refreshToken.IsUsed = true;
                _database.RefreshTokens.Update(refreshToken);
                await _database.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(refreshToken.UserId);
                return await GenerateJwtToken(user);
            }else { 
                return new UserAuthenticationResponse(){
                    result = false,
                    errors = new List<string>() { "Invalid authentication request" }
                };
            }
        }catch (Exception) {
            return new UserAuthenticationResponse(){
                result = false,
                errors = new List<string>() { "Invalid authentication request" }
            };
        }
    }

    /// <summary>
    /// This is a theaded method used to get a user.
    /// It wil return a user based on a ClaimsPrincipal class.
    /// </summary>
    /// <param name="user">A class containing user details.</param>
    /// <returns>Returns a AuthenticationUser object.</returns>
    public async Task<AuthenticationUser> getUser(ClaimsPrincipal user){
        ClaimsIdentity claimIdentity = user.Identity as ClaimsIdentity;
        if (claimIdentity.IsAuthenticated){
            string email = claimIdentity.FindFirst(ClaimTypes.Email)?.Value;
            return await _userManager.FindByEmailAsync(email);
        }
        return null;
    }

    
    /// <summary>
    /// This is a theaded method used to generate a JwtToken.
    /// It wil generate a new JwtToken based on the user.
    /// </summary>
    /// <param name="user">A class containing user details.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    public async Task<UserAuthenticationResponse> GenerateJwtToken(AuthenticationUser user){
        SecurityToken token = TokenFactory.GenerateJwtToken(_jwtConfig.Secret, user, _jwtConfig.JwtTokenLifetime); // generate JWT token
        string jwtToken = TokenFactory.WriteToken(token); // get JWT token as string
        RefreshToken refreshToken = TokenFactory.GenerateRefreshToken(token, user, _jwtConfig.RefreshTokenLifetime); // generate refresh token

        await _database.RefreshTokens.AddAsync(refreshToken);
        await _database.SaveChangesAsync();

        return new UserAuthenticationResponse(){
            token = jwtToken,
            refreshToken = refreshToken.Token,
            roles = user.roles,
            result = true,
        };
    }
}
