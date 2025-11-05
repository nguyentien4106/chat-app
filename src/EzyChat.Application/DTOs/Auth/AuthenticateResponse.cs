namespace EzyChat.Application.DTOs.Auth;

public record AuthenticateResponse(string AccessToken, string RefreshToken);

