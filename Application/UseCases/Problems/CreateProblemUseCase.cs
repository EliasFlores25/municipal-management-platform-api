using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.ProblemDtos;

namespace Application.UseCases.Problems;

public class CreateProblemUseCase
{
    private readonly IProblemRepository _problemRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProblemUseCase(
        IProblemRepository problemRepository,
        IMunicipalityRepository municipalityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _municipalityRepository = municipalityRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProblemResponse> ExecuteAsync(ProblemCreateRequest request)
    {
        var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
        if (municipality is null)
        {
            throw new ApplicationValidationException($"No se puede registrar el problema: El municipio con ID {request.MunicipalityId} no existe.");
        }

        var problem = new Problem(
            request.Title,
            request.Description,
            request.Type,
            request.Severity,
            request.MunicipalityId);

        await _problemRepository.AddAsync(problem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProblemResponse>(problem);
    }
}