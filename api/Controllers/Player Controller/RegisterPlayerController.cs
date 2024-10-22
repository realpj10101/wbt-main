using api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class RegisterPlayerController(IRegisterPlayerRepository _registerPlayerRepository) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<LoggedInDto>> Register(RegisterPlayerDto playerInput, CancellationToken cancellationToken)
    {
        if (playerInput.Password != playerInput.ConfirmPassword)
            return BadRequest("Passwords don't match");

        LoggedInDto? loggedInDto = await _registerPlayerRepository.CreateAsync(playerInput, cancellationToken);

        return !string.IsNullOrEmpty(loggedInDto.Token)
            ? Ok(loggedInDto)
            : loggedInDto.Errors.Count != 0
            ? BadRequest(loggedInDto.Errors)
            : BadRequest("Registration has failed. Try again or contact the support.");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoggedInDto>> Login(LoginDto playerInput, CancellationToken cancellationToken)
    {
        LoggedInDto? loggedInDto = await _registerPlayerRepository.LoginAsync(playerInput, cancellationToken);

        return !string.IsNullOrEmpty(loggedInDto.Token)
            ? Ok(loggedInDto)
            : loggedInDto.IsWrongCreds
            ? BadRequest("Wrong email or password.")
            : BadRequest("Registration has failed. Try again or contact the support");
    }

    [HttpGet]
    public async Task<ActionResult<LoggedInDto>> ReloadLoggedInPlayer(CancellationToken cancellationToken)
    {
        string? token = null;

        bool isTokenValid = HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);

        if (isTokenValid)
            token = authHeader.ToString().Split(' ').Last();

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is expired or invalid. Login again");

        string? hashedUserId = User.GetHashedUserId();
        if (string.IsNullOrEmpty(hashedUserId))
            return BadRequest("NO player was found with this user Id.");

        LoggedInDto? loggedInDto = await _registerPlayerRepository.ReloadLoggedInPlayerAsync(hashedUserId, token, cancellationToken);

        return loggedInDto is null ? Unauthorized("Player is logged out or unauthorized. Login again") : loggedInDto;
    }

    // [HttpGet]
    // public async Task<ActionResult<LoggedInDto?>> ReloadLoggedInPlayer(CancellationToken cancellationToken)
    // {
    //     string? tokenValue = Response.HttpContext.GetTokenAsync("acces_token").Result;

    //     LoggedInDto? loggedInDto = await _registerPlayerRepository.ReloadLoggedInPlayerAsync(User.GetPlayerId(), tokenValue, cancellationToken);

    //     return loggedInDto is not null ? loggedInDto : BadRequest("Relogin player failed");

}
