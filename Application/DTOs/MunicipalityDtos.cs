
namespace Application.DTOs
{
    public static class MunicipalityDtos
    {
        public record MunicipalityCreateRequest(string Name);
        public record MunicipalityUpdateRequest(int Id, string Name);
        public record MunicipalityResponse(int Id, string Name);
    }
}
