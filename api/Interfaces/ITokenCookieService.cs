namespace api.Interfaces;

public interface ITokenCookieService
{
    public string EncryptRefreshTokensResponse(RefreshTokenResponse refreshTokenResponse);
    public RefreshTokenRequest DecryptedRefreshTokenRequest(string protectedRefreshToken);
}