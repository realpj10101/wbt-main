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
    public string? ExperienceLevel { get; init; }
    public string? Skills { get; init; }
    public int? GamesPlayed { get; init; }
    public float? PointsPerGame { get; init; }
    public float? ReboundsPerGame { get; init; }
    public float? AssistsPerGame { get; init; }
    public string? Bio { get; init; }
    public string? Achievements { get; init; }
    public string? City { get; init; }
    public string? Region { get; init; }
    public string? Country { get; init; } 
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