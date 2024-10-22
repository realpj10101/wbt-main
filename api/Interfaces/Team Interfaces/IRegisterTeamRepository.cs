namespace api.Interfaces.Team; 

public interface IRegisterTeamRepository
{
    public Task<LoggedInTeamDto> CreateAsync(RegisterTeamDto teamInput, CancellationToken cancellationToken);    
    
    public Task<LoggedInTeamDto> LoginAsync(LoginTeamDto teamInput, CancellationToken cancellationToken);
    
    public Task<LoggedInTeamDto?> ReloadLoggedInTeamAsync(string hashedUserId, string token, CancellationToken cancellationToken);
}