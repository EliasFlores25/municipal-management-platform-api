using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.DocumentTypes
{
    public class DeleteDocumentTypeUseCase
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDocumentTypeUseCase(IDocumentTypeRepository documentTypeRepository, IUnitOfWork unitOfWork)
        {
            _documentTypeRepository = documentTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int id)
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            if (documentType is null)
            {
                throw new ApplicationValidationException($"No se puede eliminar: No existe el tipo de documento con ID {id}.");
            }

            await _documentTypeRepository.DeleteAsync(documentType);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
