using System.Runtime.InteropServices;
using api.Enums;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace api.DTOs;

public class UserUpdateDto
{
    public string? Name { get; init; }
    public string? LastName { get; init; }
    public int? Height { get; init; }
    public int? Weight { get; init; }
    public string? Gender { get; init; }
    public string? Position { get; init; }
    // public string ExperienceLevel { get; set; } = string.Empty;
    // public string Skills { get; set; } = string.Empty;
    // public int GamesPlayed { get; set; }
    // public float PointsPerGame { get; set; }
    // public float ReboundsPerGame { get; set; }
    // public float AssistsPerGame { get; set; }
    // public string
}

// public record PlayerUpdateDto(
//     [Optional][MaxLength(50)] string? Name,
//     [Optional][MaxLength(50)] string? LastName,
//     [Optional] int? Height,
//     [Optional] int? Weight,
//     [Optional][MaxLength(50)] string? Gender,
//     [Optional] string? Position,
//     [Optional][MaxLength(50)] string? ExperienceLevel,
//     [Optional][MaxLength(50)] string? Skills,
//     [Optional] int? GamesPlayed,
//     [Optional] float? PointsPerGame,
//     [Optional] float? ReboundsPerGame,
//     [Optional] float? AssistsPerGame,
//     [Optional][MaxLength(1000)] string? Bio,
//     [Optional][MaxLength(50)] string? Achievements,
//     [Optional][Length(1, 50)] string? City,
//     [Optional][Length(1, 50)] string? Region,
//     [Optional][Length(1, 50)] string? Country
// );