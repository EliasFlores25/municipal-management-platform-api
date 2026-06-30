using Domain.Enums;

namespace Application.DTOs
{
    public static class ProblemDtos
    {

        public record ProblemCreateRequest(
            string Title,
            string Description,
            ProblemType Type,
            ProblemSeverity Severity,
            int MunicipalityId);

        public record ProblemUpdateReportRequest(
            int Id,
            string Title,
            string Description,
            ProblemType Type,
            ProblemSeverity Severity);

        public record ProblemResponse(
            int Id,
            string Title,
            string Description,
            DateTime RegistrationDate,
            string Type,
            string Severity,
            string Status,
            int MunicipalityId,
            string MunicipalityName);
    }
}
