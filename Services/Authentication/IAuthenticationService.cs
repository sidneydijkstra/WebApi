using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Authentication;

/// <summary>
/// The IAuthenticationService class is an interface used as service by the application.
/// This interface can be used in different ways as Authentication service.
/// </summary>
public interface IAuthenticationService {

    /// <summary>
    /// This is a theaded method used to implement the login function.
    /// It wil login a user to the system.
    /// </summary>
    /// <param name="request">A request containing details to login a user.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    Task<UserAuthenticationResponse> login(UserLoginRequest request);

    /// <summary>
    /// This is a theaded method used to implement the regist function.
    /// It wil regist a new user to the system.
    /// </summary>
    /// <param name="request">A request containing details to regist a user.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    Task<UserAuthenticationResponse> regist(UserRegistrationRequest request);

    /// <summary>
    /// This is a theaded method used to implement the refresh function.
    /// It wil use generate a new JwtToken for the user using a RefreshToken.
    /// </summary>
    /// <param name="request">A request containing details to generate a new Token.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    Task<UserAuthenticationResponse> refresh(UserTokenRequest request);

    /// <summary>
    /// This is a theaded method used to implement the get user function.
    /// It wil return a user based on a ClaimsPrincipal class.
    /// </summary>
    /// <param name="user">A class containing user details.</param>
    /// <returns>Returns a AuthenticationUser object.</returns>
    Task<AuthenticationUser> getUser(ClaimsPrincipal user);

    /// <summary>
    /// This is a theaded method used to implement the generate a JwtToken function.
    /// It wil generate a new JwtToken.
    /// </summary>
    /// <param name="user">A class containing user details.</param>
    /// <returns>Returns a UserAuthenticationResponse object.</returns>
    Task<UserAuthenticationResponse> GenerateJwtToken(AuthenticationUser user);
}

