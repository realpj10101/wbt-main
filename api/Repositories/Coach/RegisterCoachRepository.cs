// using api.DTOs.Coach_DTOs;
// using api.Extensions;
// using api.Interfaces.Coach;
// using Microsoft.AspNetCore.Identity;
//
// namespace api.Repositories.Coach;
//
// public class RegisterCoachRepository : IRegisterCoachRepository
// {
//     #region Vars and Constructor
//
//     private readonly IMongoCollection<AppUser?> _collection;
//     private readonly UserManager<AppUser> _userManager;
//     private readonly ITokenService _tokenService;
//
//     public RegisterCoachRepository(IMongoClient client, IMyMongoDbSettings dbSettings,
//         UserManager<AppUser> userManager, ITokenService tokenService)
//     {
//         var database = client.GetDatabase(dbSettings.DatabaseName);
//         _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
//         _userManager = userManager;
//         _tokenService = tokenService;
//     }
//
//     #endregion
//
//     /// <summary>
//     /// Create an AppUser and insert in db
//     /// Check if the user doesn't already exist.
//     /// </summary>
//     /// <param name="userInput"></param>
//     /// <param name="registerCoachDto"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns>LoggedInDto</returns>
//     public async Task<LoggedInCoachDto> RegisterCoachAsync(RegisterCoachDto registerCoachDto,
//         CancellationToken cancellationToken)
//     {
//         LoggedInCoachDto loggedInCoachDto = new();
//
//         AppUser appUser = CoachMappers.ConvertRegisterCoachDtoToAppUser(registerCoachDto);
//
//         IdentityResult? coachCreatedResult = await _userManager.CreateAsync(appUser, registerCoachDto.Password);
//
//         if (coachCreatedResult.Succeeded)
//         {
//             IdentityResult? roleResult = await _userManager.AddToRoleAsync(appUser, "member");
//
//             if (!roleResult.Succeeded)
//                 return loggedInCoachDto;
//
//             string? token = await _tokenService.CreateToken(appUser, cancellationToken);
//
//             if (!string.IsNullOrEmpty(token))
//             {
//                 return CoachMappers.ConvertAppUserToLoggedInCoachDto(appUser, token);
//             }
//         }
//         else
//         {
//             foreach (IdentityError error in coachCreatedResult.Errors)
//             {
//                 loggedInCoachDto.Errors.Add(error.Description);
//             }
//         }
//
//         return loggedInCoachDto;
//     }
//
//     public async Task<LoggedInCoachDto> LoginAsync(LoginCoachDto coachInput, CancellationToken cancellationToken)
//     {
//         LoggedInCoachDto loggedInCoachDto = new();
//
//         AppUser? appUser;
//
//         appUser = await _userManager.FindByEmailAsync(coachInput.Email);
//
//         if (appUser is null)
//         {
//             loggedInCoachDto.IsWrongCreds = true;
//
//             return loggedInCoachDto;
//         }
//         
//         bool isPasswordCorrect = await _userManager.CheckPasswordAsync(appUser, coachInput.Password);
//
//         if (!isPasswordCorrect)
//         {
//             loggedInCoachDto.IsWrongCreds = true;
//
//             return loggedInCoachDto;
//         }
//
//         string? token = await _tokenService.CreateToken(appUser, cancellationToken);
//
//         if (!string.IsNullOrEmpty(token))
//         {
//             return CoachMappers.ConvertAppUserToLoggedInCoachDto(appUser, token);
//         }
//
//         return loggedInCoachDto;
//     }
//
//     public async Task<LoggedInCoachDto?> ReloadLoggedInCoachAsync(string hashedUserId, string token,
//         CancellationToken cancellationToken)
//     {
//         ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);
//
//         if (coachId is null) return null;
//         
//         AppUser appUser = await _collection.Find<AppUser>(appUser => appUser.Id == coachId).FirstOrDefaultAsync(cancellationToken);
//
//         return appUser is null
//             ? null
//             : CoachMappers.ConvertAppUserToLoggedInCoachDto(appUser, token);
//     }
//
//     public async Task<UpdateResult?> UpdateLastActive(string hashedUserId, CancellationToken cancellationToken)
//     {
//         ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);
//
//         if (coachId is null) return null;
//
//         UpdateDefinition<AppUser> newLastActive = Builders<AppUser>.Update
//             .Set(appUser => appUser.LastActive, DateTime.UtcNow);
//
//         return await _collection.UpdateOneAsync<AppUser>(coach =>
//             coach.Id == coachId, newLastActive, null, cancellationToken);
//     }
// }