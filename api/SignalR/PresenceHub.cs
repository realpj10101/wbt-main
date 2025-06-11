using api.Extensions;
using api.Hub.Interfaces;
using api.Repositories.Player;
using api.SignalR.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR;

public class PresenceHub(IPresenceTrackerService presenceTrackerService, ITokenService tokenService, IUserRepository userRepository): Microsoft.AspNetCore.SignalR.Hub
{
    public override async Task OnConnectedAsync()
    {
        ObjectId? userId =
            await tokenService.GetActualUserIdAsync(Context.User.GetHashedUserId(), GetCancellationToken())
            ?? throw new ArgumentNullException(nameof(userId), "userId is null");

        await presenceTrackerService.SaveConnectedUserAsync(userId.Value, Context.ConnectionId, GetCancellationToken());

        IEnumerable<OnlineUserDto> onlineUserDtos =
            await presenceTrackerService.GetOnlineUsersDtosAsync(GetCancellationToken());
        
        await Clients.All.SendAsync(SignalRMessages.GetOnlineUsers, onlineUserDtos, GetCancellationToken());
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        await presenceTrackerService.RemoveDisconnectedUserAsync(await GetUserName(), Context.ConnectionId);

        IEnumerable<OnlineUserDto> onlineUsersDtos = await presenceTrackerService.GetOnlineUsersDtosAsync(GetCancellationToken());
        
        await Clients.All.SendAsync(SignalRMessages.GetOnlineUsers, onlineUsersDtos);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<string> GetUserName()
    {
        string userIdHashed = Context.User?.GetHashedUserId()
                              ?? throw new HubException("The token is invalid/expired. Login again.");

        return await userRepository.GetUserNameByIdentifierHashAsync(userIdHashed, GetCancellationToken())
               ?? throw new HubException("UserName is invalid. Login again.");
    }

    private CancellationToken GetCancellationToken() =>
        Context.GetHttpContext()?.RequestAborted
        ?? throw new HubException("CancellationToken is null which cannot be.");
}