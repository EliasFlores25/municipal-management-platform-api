using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.DocumentTypeDtos;

namespace Application.UseCases.DocumentTypes
{
    public class UpdateDocumentTypeUseCase
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDocumentTypeUseCase(IDocumentTypeRepository documentTypeRepository, IUnitOfWork unitOfWork)
        {
            _documentTypeRepository = documentTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(DocumentTypeUpdateRequest request)
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(request.Id);
            if (documentType is null)
                throw new ApplicationValidationException($"No se encontró ningún tipo de documento con el ID {request.Id}.");

            var trimmedName = request.Name?.Trim() ?? string.Empty;

            if (documentType.Name != trimmedName && await _documentTypeRepository.ExistsByNameAsync(trimmedName))
                throw new ApplicationValidationException($"No se puede actualizar: El tipo de documento '{trimmedName}' ya existe.");

            documentType.UpdateDocumentType(request.Name);

            await _documentTypeRepository.UpdateAsync(documentType);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
