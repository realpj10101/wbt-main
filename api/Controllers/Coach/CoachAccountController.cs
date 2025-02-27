using api.DTOs.Coach_DTOs;
using api.Interfaces.Coach;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers.Coach;

[Authorize]
public class CoachAccountController(ICoachAccountRepository _coachAccountRepository) : BaseApiController 
{
    /// <summary>
    /// Create accounts
    /// Concurrency => async is used
    /// </summary>
    /// <param name="coachInput"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>LoggedInDto</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<LoggedInDto>> Register(AccountDto userInput,
        CancellationToken cancellationToken)
    {
        if (userInput.Password != userInput.ConfirmPassword)
            return BadRequest("Passwords don't match");
        
        LoggedInDto? loggedInDto = await _coachAccountRepository.RegisterCoachAsync(userInput, cancellationToken);

        return !string.IsNullOrEmpty(loggedInDto.Token)
            ? Ok(loggedInDto)
            : loggedInDto.Errors.Count != 0
            ? BadRequest(loggedInDto.Errors)
            : BadRequest("Registration has failed. Try again or contact the support.");
    }
}