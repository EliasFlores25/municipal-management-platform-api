using Domain;

namespace Application.Abstractions
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
