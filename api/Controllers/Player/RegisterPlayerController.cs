using api.Interfaces.Player;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

[Authorize]
public class RegisterPlayerController(IRegisterPlayerRepository _registerPlayerRepository) : BaseApiController
{
    /// <summary>
    /// Create accounts
    /// Concurrency => async is used
    /// </summary>
    /// <param name="userInput"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>LoggedInDto</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<LoggedInDto>> RegisterPlayer(RegisterPlayerDto userInput,
        CancellationToken cancellationToken)
    {
        if (userInput.Password != userInput.ConfirmPassword)
            return BadRequest("Passwords don't match");
        
        LoggedInDto? loggedInDto = await _registerPlayerRepository.RegisterPlayer(userInput, cancellationToken);
        
        return !string.IsNullOrEmpty(loggedInDto.Token)
            ? Ok(loggedInDto)
            : loggedInDto.Errors.Count != 0
            ? BadRequest(loggedInDto.Errors)
            : BadRequest("Registration has failed. Try again or contact the support.");
    }

    /// <summary>
    /// Login accounts
    /// </summary>
    /// <param name="userInput"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>LoggedInDto</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoggedInDto>> Login(LoginDto userInput, CancellationToken cancellationToken)
    {
        LoggedInDto? loggedInDto = await _registerPlayerRepository.LoginAsync(userInput, cancellationToken);

        return
            !string.IsNullOrEmpty(loggedInDto.Token)
                ? Ok(loggedInDto)
                : loggedInDto.IsWrongCreds
                ? Unauthorized("Wrong email ir password.")
                : BadRequest("Rgistration has failed. Try again or contact the support.");
    }
}