// using api.DTOs.Coach_DTOs;
// using api.Interfaces.Coach;
// using Microsoft.AspNetCore.Authorization;
//
// namespace api.Controllers.Coach;
//
// [Authorize]
// public class RegisterCoachController(IRegisterCoachRepository _registerCoachRepository) : BaseApiController 
// {
//     /// <summary>
//     /// Create accounts
//     /// Concurrency => async is used
//     /// </summary>
//     /// <param name="coachInput"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns>LoggedInDto</returns>
//     [AllowAnonymous]
//     [HttpPost("register")]
//     public async Task<ActionResult<LoggedInCoachDto>> Register(RegisterCoachDto coachInput,
//         CancellationToken cancellationToken)
//     {
//         if (coachInput.Password != coachInput.ConfirmPassword)
//             return BadRequest("Passwords don't match");
//         
//         LoggedInCoachDto? loggedInCoachDto = await _registerCoachRepository.RegisterCoachAsync(coachInput, cancellationToken);
//
//         return !string.IsNullOrEmpty(loggedInCoachDto.Token)
//             ? Ok(loggedInCoachDto)
//             : loggedInCoachDto.Errors.Count != 0
//             ? BadRequest(loggedInCoachDto.Errors)
//             : BadRequest("Registration has failed. Try again or contact the support.");
//     }
//     
//     
// }