using Application.Exceptions;
using Domain.Abstractions;
using Domain.Enums;
using MapsterMapper;
using static Application.DTOs.DocumentDtos;

namespace Application.UseCases.Documents;

public class GetDocumentQueriesUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public GetDocumentQueriesUseCase(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResponse> GetByIdAsync(int id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document is null)
        {
            throw new ApplicationValidationException($"No se encontró el documento con ID {id}.");
        }
        return _mapper.Map<DocumentResponse>(document);
    }

    public async Task<DocumentResponse> GetByDocumentNumberAsync(string documentNumber)
    {
        var normalizedNumber = documentNumber?.Trim().ToUpper() ?? string.Empty;
        var document = await _documentRepository.GetByDocumentNumberAsync(normalizedNumber);
        if (document is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún documento con el número correlativo: {normalizedNumber}.");
        }
        return _mapper.Map<DocumentResponse>(document);
    }

    public async Task<IEnumerable<DocumentResponse>> GetByMunicipalityAndStatusAsync(int municipalityId, DocumentStatus status)
    {
        var documents = await _documentRepository.GetByMunicipalityAndStatusAsync(municipalityId, status);
        return _mapper.Map<IEnumerable<DocumentResponse>>(documents);
    }

    public async Task<IEnumerable<DocumentResponse>> GetByDocumentTypeAsync(int documentTypeId)
    {
        var documents = await _documentRepository.GetByDocumentTypeAsync(documentTypeId);
        return _mapper.Map<IEnumerable<DocumentResponse>>(documents);
    }

    public async Task<IEnumerable<DocumentResponse>> GetByEmissionDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ApplicationValidationException("La fecha final de la consulta transaccional no puede ser inferior a la fecha inicial.");
        }

        var documents = await _documentRepository.GetByEmissionDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<DocumentResponse>>(documents);
    }

    public async Task<IEnumerable<DocumentResponse>> GetAllAsync()
    {
        var documents = await _documentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DocumentResponse>>(documents);
    }
}