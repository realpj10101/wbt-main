using api.Extensions;
using api.Interfaces.Team;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[Authorize]
public class RegisterTeamController(IRegisterTeamRepository _registerTeamRepository) : BaseApiController
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<LoggedInTeamDto>> Register(RegisterTeamDto teamInput, CancellationToken cancellationToken)
    {
        if (teamInput.Password != teamInput.ConfirmPassword)
            return BadRequest("Passwords dont match");

        LoggedInTeamDto? loggedInTeamDto = await _registerTeamRepository.CreateAsync(teamInput, cancellationToken);

        return !string.IsNullOrEmpty(loggedInTeamDto.Token)
            ? Ok(loggedInTeamDto)
            : loggedInTeamDto.Errors.Count != 0
            ? BadRequest(loggedInTeamDto.Errors)
            : BadRequest("Registration has failed. Try again or contact the support.");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<LoggedInTeamDto>> Login(LoginTeamDto teamInput, CancellationToken cancellationToken)
    {
        LoggedInTeamDto? loggedInTeamDto = await _registerTeamRepository.LoginAsync(teamInput, cancellationToken);

        return !string.IsNullOrEmpty(loggedInTeamDto.Token)
            ? Ok(loggedInTeamDto)
            : loggedInTeamDto.IsWrongCreds
            ? BadRequest("Wrong email or password")
            : BadRequest("Registration has failed. Try again or contact support");
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<LoggedInTeamDto>> RelaodLoggedInTeam(CancellationToken cancellationToken)
    {
        string? token = null;

        bool isTokenValid = HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader);

        if (isTokenValid)
            token = authHeader.ToString().Split(' ').Last();

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is expired or invalid. Login again.");

        string? hashedUserId = User.GetHashedUserId();
        if (string.IsNullOrEmpty(hashedUserId))
            return BadRequest("No team was found with this user Id.");

        LoggedInTeamDto? loggedInTeamDto = await _registerTeamRepository.ReloadLoggedInTeamAsync(hashedUserId, token, cancellationToken);

        return loggedInTeamDto is null ? Unauthorized("Team is logged out or unauthorized. Login Again") : loggedInTeamDto;
    }
}