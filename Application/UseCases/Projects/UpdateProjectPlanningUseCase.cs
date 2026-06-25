using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.ProjectDtos;

namespace Application.UseCases.Projects;

public class UpdateProjectPlanningUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectPlanningUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ProjectUpdatePlanningRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id);
        if (project is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún proyecto con el ID {request.Id}.");
        }

        project.UpdatePlanningDetails(request.Name, request.Description, request.EndDate, request.Budget);

        await _projectRepository.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }
}