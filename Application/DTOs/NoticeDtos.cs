using Domain.Enums;

namespace Application.DTOs
{
    public static class NoticeDtos
    {
        public record NoticeCreateRequest(
            string Title,
            string Description,
            NoticeCategory Category,
            int MunicipalityId);

        public record NoticeUpdateContentRequest(
            int Id,
            string Title,
            string Description,
            NoticeCategory Category);

        public record NoticeResponse(
            int Id,
            string Title,
            string Description,
            DateTime RegistrationDate,
            string Category,
            bool IsArchived,
            int MunicipalityId,
            string MunicipalityName);
    }
}
