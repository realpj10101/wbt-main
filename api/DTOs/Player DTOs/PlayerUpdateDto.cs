using api.Enums;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace api.DTOs;

public record PlayerUpdateDto(
    [MaxLength(50)]string? Name,
    [MaxLength(50)]string? LastName
    // int? Height,
    // int? Weight,
    // [MaxLength(50)]string? Gender,
    // string? Position,
    // [MaxLength(50)]string? ExperienceLevel,
    // [MaxLength(50)]string? Skills,
    // int? GamesPlayed,
    // float? PointsPerGame,
    // float? ReboundsPerGame,
    // float? AssistsPerGame,
    // [MaxLength(1000)]string? Bio,
    // [MaxLength(50)]string? Achievements,
    // [Length(1, 50)]string? City,
    // [Length(1, 50)]string? Region,
    // [Length(1, 50)]string? Country
);