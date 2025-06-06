namespace api.Interfaces.Teams;

public interface ITeamMessagingRepository
{
    public Task<ChatMessage?> SavedMessageAsync(MessageSenderDto message, string teamName);
    public Task<List<ChatMessage>> GetAllMessagesAsync();
}