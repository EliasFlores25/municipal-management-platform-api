using System;

namespace Application.DTOs
{
    public static class DocumentTypeDtos
    {
        public record DocumentTypeCreateRequest(string Name);
        public record DocumentTypeUpdateRequest(int Id, string Name);
        public record DocumentTypeResponse(int Id, string Name);
    }
}
