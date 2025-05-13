using System.Runtime.InteropServices;

namespace api.DTOs;

public class LoggedInDto
{
    public string? Token { get; init; }
    public string? UserName { get; init; }
    public string? Gender { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public bool IsWrongCreds { get; set; }
    public List<string> Errors { get; init; } = [];
}

// public record LoggedInDto(
//     [Optional] string? Email, // Used only to verify the account. Will always return null if the account is verified.
//     [Optional] IEnumerable<string> RolesStr,
//     [Optional] string? KnownAs,
//     [Optional] string? UserName,
//     [Optional] string? Gender,
//     [Optional] string? ProfilePhotoUrl,
//     [Optional] bool IsWrongCreds,
//     [Optional] List<string> Errors
// );
//
// public record LoginResult(
//     LoggedInDto LoggedIn,
//     [Optional] TokenDto TokenDto
// );