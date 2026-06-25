using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.DocumentTypeDtos;

namespace Application.UseCases.DocumentTypes
{
    public class GetDocumentTypeByIdUseCase
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;

        public GetDocumentTypeByIdUseCase(IDocumentTypeRepository documentTypeRepository)
            => _documentTypeRepository = documentTypeRepository;

        public async Task<DocumentTypeResponse> ExecuteAsync(int id)
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            if (documentType is null)
            {
                throw new ApplicationValidationException($"No se encontró el tipo de documento con ID {id}.");
            }

            return new DocumentTypeResponse(documentType.Id, documentType.Name);
        }
    }
}
