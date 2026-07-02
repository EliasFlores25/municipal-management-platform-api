using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.DocumentDtos;

namespace Application.UseCases.Documents;

public class CreateDocumentUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDocumentUseCase(
        IDocumentRepository documentRepository,
        IDocumentTypeRepository documentTypeRepository,
        IMunicipalityRepository municipalityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _documentTypeRepository = documentTypeRepository;
        _municipalityRepository = municipalityRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<DocumentResponse> ExecuteAsync(DocumentCreateRequest request)
    {
        var docType = await _documentTypeRepository.GetByIdAsync(request.DocumentTypeId);
        if (docType is null)
        {
            throw new ApplicationValidationException($"No se puede emitir el documento: El tipo de documento con ID {request.DocumentTypeId} no existe.");
        }

        var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
        if (municipality is null)
        {
            throw new ApplicationValidationException($"No se puede emitir el documento: El municipio con ID {request.MunicipalityId} no existe.");
        }

        var normalizedNumber = request.DocumentNumber?.Trim().ToUpper() ?? string.Empty;
        if (await _documentRepository.ExistsByDocumentNumberAsync(normalizedNumber))
        {
            throw new ApplicationValidationException($"El número de documento oficial '{normalizedNumber}' ya se encuentra registrado en el sistema.");
        }

        var document = new Document(
            request.DocumentNumber,
            request.Proprietary,
            request.Details,
            request.DocumentTypeId,
            request.MunicipalityId,
            request.EmissionDate);

        await _documentRepository.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DocumentResponse>(document);
    }
}