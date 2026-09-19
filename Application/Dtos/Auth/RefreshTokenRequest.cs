namespace Chatting.Api.Application;

public sealed record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);
