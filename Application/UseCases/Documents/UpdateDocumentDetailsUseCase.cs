using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.DocumentDtos;

namespace Application.UseCases.Documents;

public class UpdateDocumentDetailsUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentDetailsUseCase(IDocumentRepository documentRepository, IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(DocumentUpdateDetailsRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);
        if (document is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún documento con el ID {request.Id}.");
        }

        document.UpdateDetails(request.Proprietary, request.Details);

        await _documentRepository.UpdateAsync(document);
        await _unitOfWork.SaveChangesAsync();
    }
}