namespace api.Interfaces;
public interface ITokenService
{
   public Task<string?> CreateToken(RootModel rootModel, CancellationToken cancellationToken);
   public Task<ObjectId?> GetActualUserIdAsync(string? hashedUserId, CancellationToken cancellationToken);
}