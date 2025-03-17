using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.Extensions;
using Microsoft.AspNetCore.Identity;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace api.Services;

public class ExampleTokenService
{
    private readonly IMongoCollection<AppUser> _collection;
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<AppUser> _userManager;

    public ExampleTokenService(
        IConfiguration config, IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<AppUser> userManager
    )
    {
        IMongoDatabase dbName = client.GetDatabase(dbSettings.DatabaseName) ??
                                throw new ArgumentException(nameof(dbName));
        
        _collection = dbName.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);

        _jwtSettings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>()
                       ?? throw new ArgumentNullException(nameof(JwtSettings));

        _userManager = userManager;
    }

    public async Task<ObjectId?> GetActualUserIdAsync(string? userIdHashed, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userIdHashed)) return null;

        ObjectId? userId = await _collection.AsQueryable().Where(appUser => appUser.IdentifierHash == userIdHashed)
            .Select(appUser => appUser.Id).SingleOrDefaultAsync(cancellationToken);

        return ValidationsExtensions.ValidateExObjectId(userId).IsSuccess ? userId : null;
    }

    public async Task<string?> GenerateAccessTokenAsync(AppUser appUser, string identifierHash, string jtiValue)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identifierHash),
            new(JwtRegisteredClaimNames.Jti, jtiValue)
        };

        IList<string> roles = await _userManager.GetRolesAsync(appUser);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.ToUpper())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((_jwtSettings.Key)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}