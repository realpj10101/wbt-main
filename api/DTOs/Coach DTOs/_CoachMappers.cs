namespace api.DTOs.Coach_DTOs;

public static class CoachMappers
{
    public static AppUser ConvertRegisterCoachDtoToAppUser(RegisterCoachDto coachInput)
    {
        return new AppUser
        {
            Email = coachInput.Email,
            UserName = coachInput.UserName,
            Gender = coachInput.Gender
        };
    }

    public static LoggedInCoachDto ConvertAppUserToLoggedInCoachDto(AppUser appUser, string tokenValue)
    {
        return new LoggedInCoachDto
        {
            Token = tokenValue,
            UserName = appUser.NormalizedUserName,
            Gender = appUser.Gender,
            ProfilePhotoUrl = appUser.Photos.FirstOrDefault(photo => photo.IsMain)?.Url_165
        };
    }
}