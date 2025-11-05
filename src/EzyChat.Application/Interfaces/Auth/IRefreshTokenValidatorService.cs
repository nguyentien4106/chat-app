namespace EzyChat.Application.Interfaces.Auth;

public interface IRefreshTokenValidatorService
{
    bool Validate(string refreshToken);
}