namespace Chatting.Api.Application;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
