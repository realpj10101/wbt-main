using image_processing.Interfaces;
using image_processing.Services;

namespace api.Extensions;
public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IRegisterPlayerRepository, RegisterPlayerRepository>();
        services.AddScoped<IRegisterCoachRepository, RegisterCoachRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ICoachRepository, CoachRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICoachUserRepository, CoachUserRepository>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IPhotoModifySaveService, PhotoModifySaveService>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ICoachAdminRepository, CoachAdminRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IFollowCoachRepository, FollowCoachRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ILikeCoachRepository, LikeCoRepository>();

        return services;
    }
}
