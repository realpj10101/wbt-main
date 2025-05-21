namespace api.DTOs;

public record LoginDto(
    [MaxLength(50)] string Email,
    [DataType(DataType.Password)]
    [Length(PropLength.PasswordMinLength, PropLength.PasswordMaxLength)]
    string Password
);