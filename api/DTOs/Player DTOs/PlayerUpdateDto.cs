using System.Runtime.InteropServices;
using api.Enums;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace api.DTOs;

public class UserUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
    public string Gender { get; set; } = string.Empty;
    public 
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