using api.DTOs.Helpers;

namespace api.Interfaces.Teams;

public interface ITeamMessagingRepository
{
    public Task<MessageSenderDto?> SavedMessageAsync(MessageSenderDto message, string teamName);
    public Task<List<ChatMessage>> GetAllMessagesAsync();
    public Task<OperationResult> DeleteAllMessagesAsync(string teamNamem, CancellationToken cancellationToken);
}