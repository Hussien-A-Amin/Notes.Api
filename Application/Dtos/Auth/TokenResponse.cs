namespace Chatting.Api.Application;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc);