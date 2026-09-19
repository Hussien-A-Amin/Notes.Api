namespace Chatting.Api.Application;

public sealed record ConfirmEmailRequest(
    string UserId,
    string Token);
