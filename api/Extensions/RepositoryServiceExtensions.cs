using api.Interfaces.Player;
using api.Repositories.Player;
using image_processing.Interfaces;
using image_processing.Services;

namespace api.Extensions;
public static class RepositoryServiceExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        
        services.AddScoped<IRegisterPlayerRepository, RegisterPlayerRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IPhotoModifySaveService, PhotoModifySaveService>();
        

        return services;
    }
}
