using System.Security.Cryptography;
using api.Extensions;

namespace api.DTOs;

public static class Mappers
{
    public static AppUser ConvertRegisterPlayerDtoToAppUser(RegisterPlayerDto playerInput)
    {
        return new AppUser
        {
            Email = playerInput.Email,
            UserName = playerInput.UserName,
            Gender = playerInput.Gender
        };
    }

    public static LoggedInDto ConvertAppUserToLoggedInDto(AppUser appUser, string tokenValue)
    {
        return new LoggedInDto
        {
            Token = tokenValue,
            UserName = appUser.NormalizedUserName,
            KnownAs = appUser.KnownAs,
            Gender = appUser.Gender,
            ProfilePhotoUrl = appUser.Photos.FirstOrDefault(photo => photo.IsMain)?.Url_165
        };
    }

    public static PlayerDto ConvertAppUserToPlayerDto(AppUser appUser, bool isFollowing = false)
    {
        return new PlayerDto(
            // Id: rootModel.Id!.ToString(),    
            UserName: appUser.NormalizedUserName!,
            Name: appUser.Name,
            LastName: appUser.LastName,
            NationalCode: appUser.NationalCode,
            Height: appUser.Height,
            Age: CustomDateTimeExtensions.CalculateAge(appUser.Age),
            KnownAs: appUser.KnownAs,
            Created: appUser.CreatedOn,
            LastActive: appUser.LastActive,
            Gender: appUser.Gender,
            LookingFor: appUser.LookingFor,
            City: appUser.City,
            Country: appUser.Country,
            Photos: appUser.Photos,
            IsFollowing: isFollowing,
            IsCaptain: appUser.IsCaptain
        );
    }

    public static Photo ConvertPhotoUrlsToPhoto(string[] photoUrls, bool isMain)
    {
        return new Photo(
            Url_165: photoUrls[0],
            Url_256: photoUrls[1],
            Url_enlarged: photoUrls[2],
            IsMain: isMain
        );
    }

    //Follow
    public static Follow ConvertFollowIdsToFollow(ObjectId followerId, ObjectId followedId)
    {
        return new Follow(
            FollowerId: followerId,
            FollowedMemberId: followedId
        );
    }

    public static Like ConvertLikeIdsToLike(ObjectId likerId, ObjectId likedId)
    {
        return new Like(
            LikerId: likerId,
            LikedMemberId: likedId
        );
    }
}
