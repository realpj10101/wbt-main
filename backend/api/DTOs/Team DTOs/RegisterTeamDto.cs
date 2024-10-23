namespace api.DTOs;

public record RegisterTeamDto(
    [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")] string Email,
    [Length(1, 30)] string UserName,
    [DataType(DataType.Password), Length(7, 20, ErrorMessage = "Min of 7 and max of 20 chars are required.")] string Password,
    [DataType(DataType.Password), Length(7, 20)] string ConfirmPassword,
    [DataType(DataType.DateTime)] DateTime FoundedDate,
    string City,
    string Country,
    [MaxLength(60)] string Records
);

public record LoginTeamDto(
     [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")]string Email,
    [DataType(DataType.Password), Length(7, 20)] string Password
);

public class LoggedInTeamDto
{
    public string? Token { get; init; }
    public string? Email { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public bool IsWrongCreds { get; set; }
    public List<string> Errors { get; init; } = [];
}