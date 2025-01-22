namespace api.Interfaces.Player;

public interface IPlayerDetailsRepository
{
    public Task<UpdateResult?> UpdatePlayerDetailsAsync(PlayerDetailsDto playerDetailsDto, string? hashedUserId, CancellationToken cancellationToken);
}