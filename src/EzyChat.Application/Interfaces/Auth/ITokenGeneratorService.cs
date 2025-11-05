using System.Security.Claims;

namespace EzyChat.Application.Interfaces.Auth;

public interface ITokenGeneratorService
{
    string Generate(string secretKey, double expires, IEnumerable<Claim> claims = null);
}
