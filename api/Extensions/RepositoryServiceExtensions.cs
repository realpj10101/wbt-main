using api.Interfaces.Coach;
using api.Interfaces.Player;
using api.Interfaces.Teams;
using api.Repositories.Coach;
using api.Repositories.Player;
using image_processing.Interfaces;
using image_processing.Services;

namespace api.Extensions;

public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        #region Player

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        // services.AddScoped<IExAcRepo, ExAcRepo>();
        // services.AddScoped<IExampleTokenService, ExampleTokenService>();
        // services.AddScoped<IPlayerDetailsRepository, PlayerDetailsRepository>();

        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IPhotoModifySaveService, PhotoModifySaveService>();

        #endregion

        #region Coach

        services.AddScoped<ICoachAccountRepository, CoachAccountRepository>();

        #endregion

        return services;
    }
}
