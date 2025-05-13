namespace api.DTOs;

public enum ErrorCode
{
    IsWrongCreds,
    IsAccountCreationFailed,
    NetIdentityFailed,
    CoachNotFound,
    CoachHasNoTeam,
    OnlyOneCaptain,
    UserNotFound,
    NotInTeam,
    NotTeamMember,
    AlreadyCaptain,
    IsRefreshTokenExpired,
    IsSessionExpired,
    TeamNotFound,
    IsNotCaptain
}