using api.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class RegisterCoachController(IRegisterCoachRepository _registerCoachRepository) : BaseApiController
{

    [AllowAnonymous]
    [HttpPost("register-coach")]
    public async Task<ActionResult<LoggedInCoachDto>> Register(RegisterCoachDto coachInput, CancellationToken cancellationToken)
    {
        if (coachInput.Password != coachInput.ConfirmPassword)
            return BadRequest("Passwords don't match");

       LoggedInCoachDto? loggedInCoachDto = await _registerCoachRepository.CreateCoachAsync(coachInput, cancellationToken);

       return loggedInCoachDto.Token is null ? BadRequest("Registration has failed. Try again.") : loggedInCoachDto;
    }

    [AllowAnonymous]
    [HttpPost("login-coach")]
    public async Task<ActionResult<LoggedInCoachDto>> Login(LoginCoachDto coachInput, CancellationToken cancellationToken)
    {
        LoggedInCoachDto? loggedInCoachDto = await _registerCoachRepository.LoginCoachAsync(coachInput, cancellationToken);

        if (loggedInCoachDto.IsWrongCreds) return Unauthorized("Invalid username or password");

        return loggedInCoachDto.Token is null ? BadRequest("Login has failed. Try again.") : loggedInCoachDto;
    }

    [Authorize]
    [HttpGet]
    public ActionResult AuthorizeLoggedInCoach() =>
        Ok(new { message = "token is still valid and coach is authorized"});

    // [HttpGet]
    // public async Task<ActionResult<LoggedInCoachDto?>> ReloadLoggedInCoach(CancellationToken cancellationToken)
    // {
    //     string? tokenValue = Response.HttpContext.GetTokenAsync("access_token").Result;

    //     LoggedInCoachDto? loggedInCoachDto = await _registerCoachRepository.ReloadLoggedInCoachAsync(User.GetCoachId(), tokenValue, cancellationToken);

    //     return loggedInCoachDto is null ? loggedInCoachDto : BadRequest("Relogin coach  failed");
    // }
}
