namespace api.DTOs;

public record RefreshTokenResponse(
    string TokenValueRaw,
    string JtiValue,
    DateTimeOffset ExpiresAt
);