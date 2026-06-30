using Application.Exceptions;
using Domain.Abstractions;
using Domain.Enums;
using MapsterMapper;
using static Application.DTOs.ProblemDtos;

namespace Application.UseCases.Problems;

public class GetProblemQueriesUseCase
{
    private readonly IProblemRepository _problemRepository;
    private readonly IMapper _mapper;

    public GetProblemQueriesUseCase(IProblemRepository problemRepository, IMapper mapper)
    {
        _problemRepository = problemRepository;
        _mapper = mapper;
    }

    public async Task<ProblemResponse> GetByIdAsync(int id)
    {
        var problem = await _problemRepository.GetByIdAsync(id);
        if (problem is null)
        {
            throw new ApplicationValidationException($"No se encontró el reporte con ID {id}.");
        }
        return _mapper.Map<ProblemResponse>(problem);
    }

    public async Task<IEnumerable<ProblemResponse>> GetAllAsync()
    {
        var problems = await _problemRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProblemResponse>>(problems);
    }

    public async Task<IEnumerable<ProblemResponse>> GetByMunicipalityAsync(int municipalityId)
    {
        var problems = await _problemRepository.GetByMunicipalityAsync(municipalityId);
        return _mapper.Map<IEnumerable<ProblemResponse>>(problems);
    }

    public async Task<IEnumerable<ProblemResponse>> GetByMunicipalityAndStatusAsync(int municipalityId, ProblemStatus status)
    {
        var problems = await _problemRepository.GetByMunicipalityAndStatusAsync(municipalityId, status);
        return _mapper.Map<IEnumerable<ProblemResponse>>(problems);
    }

    public async Task<IEnumerable<ProblemResponse>> GetBySeverityAndStatusAsync(ProblemSeverity severity, ProblemStatus status)
    {
        var problems = await _problemRepository.GetBySeverityAndStatusAsync(severity, status);
        return _mapper.Map<IEnumerable<ProblemResponse>>(problems);
    }

    public async Task<IEnumerable<ProblemResponse>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ApplicationValidationException("La fecha final de la consulta no puede ser menor a la fecha inicial.");
        }

        var problems = await _problemRepository.GetByRegistrationDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<ProblemResponse>>(problems);
    }
}