namespace api.DTOs;

public record CoachUpdateDto(
    [MaxLength(1000)]string? LookingFor,
    [Length(2, 30)]string City,
    [Length(2, 30)]string Country,
    [MaxLength(1000)]string Record
);