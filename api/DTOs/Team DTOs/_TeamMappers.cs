namespace api.DTOs.Team; 

public static class TeamMappers
{   
    public static RootModel ConvertRegisterTeamDtoToRootModel(RegisterTeamDto teamInput)
    {
        return new RootModel
        {
            Email = teamInput.Email,
            UserName = teamInput.UserName,
            FoundedDate = teamInput.FoundedDate,
            City = teamInput.City,
            Country = teamInput.Country,
            Records = teamInput.Records       
        };
    }

    public static LoggedInTeamDto ConvertRootModelToLoggedInTeamDto(RootModel rootModel, string tokenValue)
    {
        return new LoggedInTeamDto
        {
            Email = rootModel.Email,
            Token = tokenValue,
            ProfilePhotoUrl = rootModel.Photos.FirstOrDefault(photo => photo.IsMain)?.Url_165
        };
    }

    public static TeamDto ConvertRootModelToTeamDto(RootModel rootModel, bool isJoining = false)
    {
        return new TeamDto(
            UserName: rootModel.NormalizedUserName!,
            FoundedDate: rootModel.FoundedDate,
            NumberOfGames: rootModel.NumberOfGames,
            NumberOfWins: rootModel.NumberOfWins,
            NumberOfLosses: rootModel.NumberOfLosses,
            GameHistory: rootModel.GameHistory,
            GameResults: rootModel.GameResults,
            City: rootModel.City,
            Country: rootModel.Country,
            Records: rootModel.Records,
            Photos: rootModel.Photos,
            IsJoining: isJoining
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

    public static Join ConvertJoinsIdsToJoin(ObjectId joinerId, ObjectId joinedId)
    {
        return new Join(
            JoinerId: joinerId,
            JoinedTeamId: joinedId
        );
    }
}