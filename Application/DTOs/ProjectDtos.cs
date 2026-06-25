
namespace Application.DTOs
{
    public static class ProjectDtos
    {
        public record ProjectCreateRequest(
            string Name,
            string Description,
            DateTime EndDate,
            decimal Budget,
            int MunicipalityId);

        public record ProjectUpdatePlanningRequest(
            int Id,
            string Name,
            string Description,
            DateTime EndDate,
            decimal Budget);

        public record ProjectResponse(
            int Id,
            string Name,
            string Description,
            DateTime StartDate,
            DateTime EndDate,
            decimal Budget,
            string State,
            int MunicipalityId,
            string MunicipalityName);
    }
}
