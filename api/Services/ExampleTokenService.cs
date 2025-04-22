// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using api.Extensions;
// using api.Models.Helpers;
// using Microsoft.AspNetCore.Identity;
// using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
//
// namespace api.Services;
//
// public class ExampleTokenService : IExampleTokenService
// {
//     private readonly IMongoCollection<AppUser> _collection;
//     private readonly IMongoCollection<RefreshToken> _collectionRefreshTokens;
//     private readonly JwtSettings _jwtSettings;
//     private readonly UserManager<AppUser> _userManager;
//
//     public ExampleTokenService(
//         IConfiguration config, IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<AppUser> userManager
//     )
//     {
//         IMongoDatabase dbName = client.GetDatabase(dbSettings.DatabaseName) ??
//                                 throw new ArgumentException(nameof(dbName));
//
//         _collection = dbName.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);
//         _collectionRefreshTokens = dbName.GetCollection<RefreshToken>(AppVariablesExtensions.CollectionRefreshTokens);
//
//         _jwtSettings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>()
//                        ?? throw new ArgumentNullException(nameof(JwtSettings));
//
//         _userManager = userManager;
//     }
//
//     public async Task<TokenDto> GenerateTokensAsync(
//         RefreshTokenRequest refreshTokenRequest, AppUser appUser, CancellationToken cancellationToken
//     )
//     {
//         string identifierHash = await StoreHashedUserId(
//             appUser.Id, cancellationToken
//         ); // this securedId is stored in user's collection to associate with the AppUser.
//
//         return new TokenDto(
//             await GenerateAccessTokenAsync(identifierHash, appUser),
//             await GenerateRefreshTokenAsync(refreshTokenRequest, appUser.Id, cancellationToken)
//         );
//     }
//
//     public async Task<ObjectId?> GetActualUserIdAsync(string? userIdHashed, CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrEmpty(userIdHashed)) return null;
//
//         ObjectId? userId = await _collection.AsQueryable().Where(appUser => appUser.IdentifierHash == userIdHashed)
//             .Select(appUser => appUser.Id).SingleOrDefaultAsync(cancellationToken);
//
//         return ValidationsExtensions.ValidateExObjectId(userId).IsSuccess ? userId : null;
//     }
//
//     private async Task<string> GenerateAccessTokenAsync(string identifierHash, AppUser appUser)
//     {
//         var claims = new List<Claim>
//         {
//             new(JwtRegisteredClaimNames.Sub, identifierHash),
//         };
//
//         IList<string> roles = await _userManager.GetRolesAsync(appUser);
//         claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.ToUpper())));
//
//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((_jwtSettings.Key)));
//         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
//
//         var token = new JwtSecurityToken(
//             _jwtSettings.Issuer,
//             _jwtSettings.Audience,
//             claims,
//             expires: DateTime.UtcNow.AddMinutes(10),
//             signingCredentials: creds
//         );
//
//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }
//
//     private async Task<RefreshTokenResponse> GenerateRefreshTokenAsync(
//         RefreshTokenRequest refreshTokenRequest, ObjectId userId, CancellationToken cancellationToken
//     )
//     {
//         var tokenValueRaw = Guid.CreateVersion7().ToString();
//
//         var refreshToken = new RefreshToken
//         {
//             UserId = userId,
//             JtiValue = Guid.CreateVersion7().ToString(),
//             TokenValueHashed = TokenHasher.HashWithSecret(tokenValueRaw, _jwtSettings.Key),
//             CreatedAt = DateTimeOffset.UtcNow,
//             ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
//             SessionMetadata = refreshTokenRequest.SessionMetadata
//         };
//
//         await _collectionRefreshTokens.InsertOneAsync(refreshToken, null, cancellationToken);
//
//         return new RefreshTokenResponse(
//             tokenValueRaw,
//             refreshToken.JtiValue,
//             refreshToken.ExpiresAt
//         );
//     }
//
//     private async Task<string> StoreHashedUserId(
//         ObjectId userId, CancellationToken cancellationToken
//     )
//     {
//         var identifierHash = Guid.CreateVersion7().ToString();
//
//         UpdateDefinition<AppUser> updateIdentifierHash = Builders<AppUser>.Update
//             .Set(appUser => appUser.IdentifierHash, identifierHash);
//
//         UpdateResult updateResult =
//             await _collection.UpdateOneAsync(doc => doc.Id == userId, updateIdentifierHash, null, cancellationToken);
//
//         return updateResult.ModifiedCount == 1
//             ? identifierHash
//             : throw new ApplicationException("Update identifier hash to DB failed.");
//     }
// }