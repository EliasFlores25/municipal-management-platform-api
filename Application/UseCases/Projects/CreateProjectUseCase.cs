using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.ProjectDtos;

namespace Application.UseCases.Projects;

public class CreateProjectUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectUseCase(
        IProjectRepository projectRepository,
        IMunicipalityRepository municipalityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _municipalityRepository = municipalityRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectResponse> ExecuteAsync(ProjectCreateRequest request)
    {
        var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
        if (municipality is null)
        {
            throw new ApplicationValidationException($"No se puede registrar el proyecto: El municipio con ID {request.MunicipalityId} no existe.");
        }

        var project = new Project(
            request.Name,
            request.Description,
            request.EndDate,
            request.Budget,
            request.MunicipalityId);

        await _projectRepository.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }
}