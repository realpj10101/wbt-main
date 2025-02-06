using api.Extensions;
using api.Interfaces.Player;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Player;

[Authorize]
public class AccountController(IAccountRepository _accountRepository) : BaseApiController
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
    public async Task<ActionResult<LoggedInDto>> Register(AccountDto userInput,
        CancellationToken cancellationToken)
    {
        if (userInput.Password != userInput.ConfirmPassword)
            return BadRequest("Passwords don't match");
        
        LoggedInDto? loggedInDto = await _accountRepository.RegisterPlayerAsync(userInput, cancellationToken);
        
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
        LoggedInDto? loggedInDto = await _accountRepository.LoginAsync(userInput, cancellationToken);

        return  
            !string.IsNullOrEmpty(loggedInDto.Token)
                ? Ok(loggedInDto)
                : loggedInDto.IsWrongCreds
                ? Unauthorized("Wrong email ir password.")
                : BadRequest("Registration has failed. Try again or contact the support.");
    }
    
    [HttpGet]
    public async Task<ActionResult<LoggedInDto>> ReloadLoggedInUser(CancellationToken cancellationToken)
    {
        // obtain token value
        string? token = null;
        
        bool isTokenValid = HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);

        if (isTokenValid)
            token = authHeader.ToString().Split(' ').Last();

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is expired or invalid. Login again.");
        
        // obtain userId
        string? hashedUserId = User.GetHashedUserId();
        if (string.IsNullOrEmpty(hashedUserId))
            return BadRequest("No user found with this user Id");

        // get loggedInDto
        LoggedInDto? loggedInDto =
            await _accountRepository.ReloadLoggedInUserAsync(hashedUserId, token, cancellationToken);

        return loggedInDto is null ? Unauthorized("User is logged out or unauthorized. Login again") : loggedInDto;
    }
}