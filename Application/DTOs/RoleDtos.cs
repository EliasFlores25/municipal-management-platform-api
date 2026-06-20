
namespace Application.DTOs
{
    public static class RoleDtos
    {
        public record CreateRequest(string Name);
        public record Response(int Id, string Name);
    }
}
