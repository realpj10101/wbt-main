using api.DTOs.Helpers;
using api.DTOs.Team_DTOs;
using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces.Teams;

public interface ITeamRepository
{
    public Task<ShowTeamDto?> CreateAsync(ObjectId userId, CreateTeamDto userInput, CancellationToken cancellationToken);
    // public Task<TeamStatus> UpdateTeamAsync(ObjectId userId, UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken);
    public Task<PagedList<Team>?> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
    public Task<ShowTeamDto?> GetByTeamNameAsync(string teamName, CancellationToken cancellationToken);
    public Task<List<AppUser>?> GetTeamMembersAsync(string teamName,  CancellationToken cancellationToken);
    public Task<TeamStatus> AddMemberAsync(ObjectId userId, string targetMemberUserName, string targetTeamName,
        CancellationToken cancellationToken);

    public Task<TeamStatus> RemoveMemberAsync(ObjectId userId, string targetMemberUserName, string targetTeamName,
        CancellationToken cancellationToken
    );
    public Task<string?> GetTeamNameByIdAsync(ObjectId userId, CancellationToken cancellationToken);
    public Task<OperationResult> AssignCaptainAsync(ObjectId coachId, string targetUserName, CancellationToken cancellationToken);
    public Task<OperationResult> RemoveCaptainAsync(ObjectId coachId, string targetUserName, CancellationToken cancellationToken);
    public Task<OperationResult> RequestJoinTeamAsync(string teamName, ObjectId playerId, CancellationToken cancellationToken);
    public Task<Team?> GetByIdAsync(ObjectId teamId, CancellationToken cancellationToken);
    public Task<Photo?> UploadPhotoAsync(IFormFile file, string? hashedUserId, string teamName, CancellationToken cancellationToken);
    public Task<UpdateResult?> SetMainPhotoAsync(string hashedUserId, string teamName, string photoUrlIn, CancellationToken cancellationToken);
    public Task<UpdateResult?> DeletePhotoAsync(string hashedUserId, string teamName, string? photoUrlIn, CancellationToken cancellationToken);
}