
namespace Application.DTOs
{
    public static class PositionDtos
    {
        public record PositionCreateRequest(string Name, string Description);
        public record PositionUpdateRequest(int Id, string Name, string Description);
        public record PositionResponse(int Id, string Name, string Description);
    }
}
