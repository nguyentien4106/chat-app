using System.Security.Claims;

namespace ChatApp.Application.Interfaces.Auth;

public interface ITokenGeneratorService
{
    string Generate(string secretKey, double expires, IEnumerable<Claim> claims = null);
}
