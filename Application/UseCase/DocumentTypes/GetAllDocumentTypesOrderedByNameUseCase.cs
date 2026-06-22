using Domain.Abstractions;
using static Application.DTOs.DocumentTypeDtos;

namespace Application.UseCases.DocumentTypes
{
    public class GetAllDocumentTypesOrderedByNameUseCase
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;

        public GetAllDocumentTypesOrderedByNameUseCase(IDocumentTypeRepository documentTypeRepository)
            => _documentTypeRepository = documentTypeRepository;
        public async Task<IEnumerable<DocumentTypeResponse>> ExecuteAsync()

            => (await _documentTypeRepository.GetAllOrderedByNameAsync())
                                             .Select(dt => new DocumentTypeResponse(dt.Id, dt.Name));
    }
}
