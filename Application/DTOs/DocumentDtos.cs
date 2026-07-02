
namespace Application.DTOs
{
    public static class DocumentDtos
    {
        public record DocumentCreateRequest(
            string DocumentNumber,
            string Proprietary,
            string Details,
            int DocumentTypeId,
            int MunicipalityId,
            DateTime? EmissionDate);

        public record DocumentUpdateDetailsRequest(
            int Id,
            string Proprietary,
            string Details);

        public record DocumentResponse(
            int Id,
            string DocumentNumber,
            DateTime EmissionDate,
            string Proprietary,
            string Details,
            string State,
            int DocumentTypeId,
            string DocumentTypeName,
            int MunicipalityId,
            string MunicipalityName);

    }
}
