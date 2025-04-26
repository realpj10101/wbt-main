using System.Text.Json;
using api.DTOs.Helpers;
using api.Extensions;
using api.Interfaces.Player;
using api.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using UAParser;
using UAParser.Objects;

namespace api.Controllers.Player;

[Authorize]
public class AccountController(
    IAccountRepository _accountRepository
    // ITokenCookieService tokenCookieService
) : BaseApiController
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
    public async Task<ActionResult<LoggedInDto>> Register(RegisterDto userInput,
        CancellationToken cancellationToken)
    {
        if (userInput.Password != userInput.ConfirmPassword)
            return BadRequest("Passwords don't match");

        OperationResult<LoggedInDto> opResult =
            await _accountRepository.RegisterPlayerAsync(userInput, cancellationToken);

        return opResult.IsSuccess
            ? Ok(opResult.Result)
            : opResult.Error?.Code switch
            {
                ErrorCode.NetIdentifyFailed => BadRequest(opResult.Error.Message),
                ErrorCode.IsAccountCreationFailed => BadRequest(opResult.Error.Message),
                _ => BadRequest("Creating account failed. Contact administrator")
            };
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

    // [HttpPost("refresh-tokens")]
    // public async Task<ActionResult> RefreshTokens(CancellationToken cancellationToken)
    // {
    //     string? identifierHash = User.GetHashedUserId();
    //     if (string.IsNullOrEmpty(identifierHash))
    //         return Unauthorized("Your not logged in. Please login again.");
    //     
    //     // OperationResult<TokenDto> 
    //     //     opResult = await _accountRepository.RefreshTokensAsync(identifierHash, cancellationToken);
    //     
    //     OperationResult<TokenDto> 
    //         opResult = await _acRepo.RefreshTokensAsync(identifierHash, cancellationToken);
    //
    //     if (!opResult.IsSuccess)
    //     {   
    //         return opResult.Error.Code switch
    //         {
    //         ErrorCode.IsRefreshTokenExpired => Unauthorized(opResult.Error.Message),
    //         _ => BadRequest("Failed to refresh token. Try again or contact the support.")
    //         };
    //     }
    //     
    //     AddTokensToResponseCookies(opResult.Result);
    //     return Ok("Tokens refreshed successfully.");
    // }

    // private void AddTokensToResponseCookies(TokenDto tokenDto)
    // {
    //     Response.Cookies.Delete(
    //         "auth-access-token", new CookieOptions
    //         {
    //             Path = "/"
    //         }
    //     );
    //
    //     Response.Cookies.Delete(
    //         "auth-refresh-token", new CookieOptions
    //         {
    //             Path = "/api/account/refresh-tokens"
    //         }
    //     );
    //
    //     Response.Cookies.Append(
    //         "auth-access_token", tokenDto.AccessToken, new CookieOptions
    //         {
    //             HttpOnly = true,
    //             Secure = true,
    //             SameSite = SameSiteMode.None, // Lax for production
    //             Expires = CustomDateTimeExtensions.GetTokenExpirationDate(tokenDto.AccessToken),
    //             Path = "/"
    //         }
    //     );
    //
    //     string encryptedCookie = tokenCookieService.EncryptRefreshTokensResponse(tokenDto.RefreshToken);
    //
    //     Response.Cookies.Append(
    //         "refresh_token", encryptedCookie, new CookieOptions
    //         {
    //             HttpOnly = true,
    //             Secure = true,
    //             SameSite = SameSiteMode.None,
    //             Expires = tokenDto.RefreshToken.ExpiresAt.UtcDateTime,
    //             Path = "/api/account/refresh-token"
    //         }
    //     );
    // }

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

    private async Task<SessionMetadata> ExtractSessionMetaData()
    {
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        Parser? parser = Parser.GetDefault();
        ClientInfo? client = parser.Parse(userAgent);

        string deviceType = client.Device.IsSpider ? "Bot" :
            string.IsNullOrWhiteSpace(client.Device.Family) ? "Unknown" : client.Device.Family;
        var os = $"{client.OS.Family} {client.OS.Major}";
        var browser = $"{client.Browser.Family} {client.Browser.Major}";
        
        var deviceName = $"{os} - {browser}";
        
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // get location from headers (behind Cloudflare)
        // string location = HttpContext.Request.Headers["CF-IPCountry"].FirstOrDefault() ?? "Unknown";

        string location = await GetCountryFromIp(ipAddress);
        
        return new SessionMetadata(
            deviceType,
            deviceName,
            string.IsNullOrWhiteSpace(userAgent) ? "Unknown" : userAgent,
            ipAddress,
            location
        );
    }

    private async Task<string> GetCountryFromIp(string ip)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ip}");
        using var doc = JsonDocument.Parse(response);
        return doc.RootElement.GetProperty("country").GetString() ?? "unknown";
    }
}