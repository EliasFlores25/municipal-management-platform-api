using Application.Exceptions;
using Domain.Abstractions;
using Domain.Enums;
using MapsterMapper;
using static Application.DTOs.ProjectDtos;

namespace Application.UseCases.Projects;

public class GetProjectQueriesUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetProjectQueriesUseCase(IProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<ProjectResponse> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún proyecto con el ID {id}.");
        }
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<IEnumerable<ProjectResponse>> GetByMunicipalityAsync(int municipalityId)
    {
        var projects = await _projectRepository.GetByMunicipalityAsync(municipalityId);
        return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
    }

    public async Task<IEnumerable<ProjectResponse>> GetByMunicipalityAndStatusAsync(int municipalityId, ProjectStatus state)
    {
        var projects = await _projectRepository.GetByMunicipalityAndStatusAsync(municipalityId, state);
        return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
    }

    public async Task<IEnumerable<ProjectResponse>> GetProjectsWithHighBudgetAsync(decimal minBudget)
    {
        var projects = await _projectRepository.GetProjectsWithHighBudgetAsync(minBudget);
        return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
    }

    public async Task<IEnumerable<ProjectResponse>> GetOverdueProjectsAsync(DateTime? referenceDate = null)
    {
        var dateToCompare = referenceDate ?? DateTime.UtcNow;

        var projects = await _projectRepository.GetOverdueProjectsAsync(dateToCompare);
        return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
    }
    public async Task<IEnumerable<ProjectResponse>> GetAllOrderedByNameAsync()
    {
        var projects = await _projectRepository.GetAllOrderByNameAsync();
        return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
    }
}