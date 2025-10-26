namespace ChatApp.Application.DTOs.Auth;

public record AuthenticateResponse(string AccessToken, string RefreshToken);

