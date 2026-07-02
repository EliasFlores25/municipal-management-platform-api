using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.Documents;

public class AnularDocumentUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnularDocumentUseCase(IDocumentRepository documentRepository, IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún documento con el ID {id}.");
        }

        document.Anular();

        await _documentRepository.UpdateAsync(document);
        await _unitOfWork.SaveChangesAsync();
    }
}