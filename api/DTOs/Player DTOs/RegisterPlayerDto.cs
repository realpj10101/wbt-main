namespace api.DTOs;

public record RegisterPlayerDto(
    [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")] string Email,
    [Length(1, 30)] string UserName,
    [DataType(DataType.Password), Length(7, 20, ErrorMessage = "Min of 7 and max of 20 chars are required.")] string Password,
    [DataType(DataType.Password), Length(7, 20)] string ConfirmPassword,
    [MinLength(2), MaxLength(20)] string Name,
    [MinLength(2), MaxLength(30)] string LastName,
    [MinLength(10), MaxLength(10)] string NationalCode,
    int Height,
    [Range(typeof(DateOnly), "1900-01-01", "2050-01-01")] DateOnly Age,
    [Length(1, 30)] string KnownAs,
    [Length(3, 20)] string Gender,
    [Length(2, 20)] string City,
    [Length(3, 20)] string Country
);

public record LoginDto(
      [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")] string Email,
      [DataType(DataType.Password), MinLength(7), MaxLength(20)] string Password
);

public class LoggedInDto
{
    public string? Token { get; init; }
    public string? UserName { get; init; }
    public string? KnownAs { get; init; }
    public string? Gender { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public bool IsWrongCreds { get; set; }
    public List<string> Errors { get; init; } = [];
}