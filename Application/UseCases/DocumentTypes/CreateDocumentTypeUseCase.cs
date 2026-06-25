using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using static Application.DTOs.DocumentTypeDtos;

namespace Application.UseCases.DocumentTypes
{
    public class CreateDocumentTypeUseCase
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDocumentTypeUseCase(IDocumentTypeRepository documentTypeRepository, IUnitOfWork unitOfWork)
        {
            _documentTypeRepository = documentTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentTypeResponse> ExecuteAsync(DocumentTypeCreateRequest request)
        {
            var trimmedName = request.Name?.Trim() ?? string.Empty;

            if (await _documentTypeRepository.ExistsByNameAsync(trimmedName))
            {
                throw new ApplicationValidationException($"El tipo de documento '{trimmedName}' ya se encuentra registrado.");
            }

            var documentType = new DocumentType(request.Name);

            await _documentTypeRepository.AddAsync(documentType);
            await _unitOfWork.SaveChangesAsync();

            return new DocumentTypeResponse(documentType.Id, documentType.Name);
        }
    }
}
