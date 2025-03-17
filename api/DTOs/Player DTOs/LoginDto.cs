namespace api.DTOs;

public record LoginDto(
    [MaxLength(PropLength.EmailMaxLength)]
    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$", ErrorMessage = "Bad Email Format.")]
    string Email,
    [DataType(DataType.Password)]
    [MinLength(PropLength.PasswordsMinLength), MaxLength(PropLength.PasswordsMaxLength)]
    string Password
);