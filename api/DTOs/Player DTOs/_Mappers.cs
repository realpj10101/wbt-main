using System.Security.Cryptography;
using api.Extensions;

namespace api.DTOs;

public static class Mappers
{
    public static RootModel ConvertRegisterPlayerDtoToRootModel(RegisterPlayerDto playerInput)
    {

        return new RootModel
        {
            Email = playerInput.Email,
            UserName = playerInput.UserName,
            Name = playerInput.Name,
            LastName = playerInput.LastName,
            NationalCode = playerInput.NationalCode.Trim(),
            Height = playerInput.Height,
            Age = playerInput.Age,
            KnownAs = playerInput.KnownAs.Trim(),
            LastActive = DateTime.UtcNow,
            Gender = playerInput.Gender.ToLower(),
            City = playerInput.City.Trim(),
            Country = playerInput.Country.Trim(),
            Photos = []
        };
    }

    public static LoggedInDto ConvertRootModelToLoggedInDto(RootModel rootModel, string tokenValue)
    {
        return new LoggedInDto
        {
            Token = tokenValue,
            UserName = rootModel.NormalizedUserName,
            KnownAs = rootModel.KnownAs,
            Gender = rootModel.Gender,
            ProfilePhotoUrl = rootModel.Photos.FirstOrDefault(photo => photo.IsMain)?.Url_165
        };
    }

    public static PlayerDto ConvertRootModelToPlayerDto(RootModel rootModel, bool isFollowing = false)
    {
        return new PlayerDto(
            // Id: rootModel.Id!.ToString(),    
            UserName: rootModel.NormalizedUserName!,
            Name: rootModel.Name,
            LastName: rootModel.LastName,
            NationalCode: rootModel.NationalCode,
            Height: rootModel.Height,
            Age: CustomDateTimeExtensions.CalculateAge(rootModel.Age),
            KnownAs: rootModel.KnownAs,
            Created: rootModel.CreatedOn,
            LastActive: rootModel.LastActive,
            Gender: rootModel.Gender,
            LookingFor: rootModel.LookingFor,
            City: rootModel.City,
            Country: rootModel.Country,
            Photos: rootModel.Photos,
            IsFollowing: isFollowing,
            IsCaptain: rootModel.IsCaptain
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
