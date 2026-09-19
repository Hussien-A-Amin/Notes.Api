using Azure.Core;
using Chatting.Api.Application;
using Chatting.Api.Application.Services;
using Chatting.Api.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace Chatting.Api.Controllers;

public partial class AuthController
{
    TokenResponse GetTokenResponse()
    {
        var ExpiresAt = DateTime.Now.AddMinutes(10);

        var clms = new List<Claim>()
            {
                new ("BonusRole","BonusManager"),
                new (ClaimTypes.Role,"Manager"),
            };


        var jwthandler = new JwtSecurityTokenHandler();
        var token = jwthandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(clms)
            ,
            Issuer = jwtOptions.Issuer
            ,
            Audience = jwtOptions.Audience
            ,
            Expires = ExpiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256Signature)
        });



        // refresh Token
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        var refreshToken = Convert.ToBase64String(randomBytes);





       return new TokenResponse(
            jwthandler.WriteToken(token),
            refreshToken,
            ExpiresAt);



    }

}


[ApiController]
[Route("[controller]")]
public partial class AuthController(

    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    
    JwtOptions jwtOptions
    ) 
    : ControllerBase
{
    // POST: api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        
        //Register
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            
               return   BadRequest( result.Errors.Select(x => x.Description));
        }

       
  

        return Ok();
    }


    // POST: api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return BadRequest("Invalid email or password");


        var result = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
            return BadRequest("Invalid email or password");




        var rs = GetTokenResponse();

        return Ok(rs);
    }




    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        //var result = await tokenService.RefreshTokenAsync(
        //    request,
        //    cancellationToken);

        return Ok("result");
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Headers["X-Refresh-Token"]
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest("Refresh token is required.");

        //await tokenService.RevokeRefreshTokenAsync(
        //    refreshToken,
        //    cancellationToken);

        return NoContent();
    }


    // POST: api/auth/forgot-password
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        //await authenticationService.ForgotPasswordAsync(
        //    request,
        //    cancellationToken);

        return Ok(new
        {
            message = "If the account exists, a password reset link has been sent."
        });
    }


    // POST: api/auth/reset-password
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        //await authenticationService.ResetPasswordAsync(
        //    request,
        //    cancellationToken);

        return Ok(new
        {
            message = "Password has been reset successfully."
        });
    }


    // POST: api/auth/confirm-email
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        //await authenticationService.ConfirmEmailAsync(
        //    request,
        //    cancellationToken);

        return Ok(new
        {
            message = "Email confirmed successfully."
        });
    }


    // POST: api/auth/change-password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        //await authenticationService.ChangePasswordAsync(
        //    userId,
        //    request,
        //    cancellationToken);

        return Ok(new
        {
            message = "Password changed successfully."
        });
    }


}