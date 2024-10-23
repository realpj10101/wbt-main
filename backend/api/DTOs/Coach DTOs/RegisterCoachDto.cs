namespace api.DTOs;
public record RegisterCoachDto(
    [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")] string Email,
    [Length(1, 30)] string UserName,
    [DataType(DataType.Password), MinLength(7), MaxLength(20)] string Password,
    [DataType(DataType.Password), MinLength(7), MaxLength(20)] string ConfirmPassword,
    [MinLength(2), MaxLength(20)] string Name,
    [MinLength(2), MaxLength(30)] string LastName,
    [MinLength(10), MaxLength(10)] string NationalCode,
    [Length(2, 30)] string KnownAs,
    [Range(typeof(DateOnly), "1900-01-01", "2050-01-01")] DateOnly Age,
    [Length(3, 20)] string Gender,
    [Length(3, 20)] string City,
    [Length(3, 20)] string Country,
    [MaxLength(60)] string Records
);

public record LoginCoachDto(
    [MaxLength(50), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")] string Email,
    [DataType(DataType.Password), MinLength(7), MaxLength(20)] string Password
);

public class LoggedInCoachDto
{
    public string? Email { get; set; }
    public string? Token { get; set; }
    public string? KnownAs { get; set; }
    public string? Gender { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public bool IsWrongCreds { get; set; }
    public List<string> Errors { get; set; } = [];
};