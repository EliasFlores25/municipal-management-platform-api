using Domain.Abstractions;
using Application.Exceptions;
using Domain;

namespace Application.UseCases.Projects;

public class ChangeProjectStatusUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeProjectStatusUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task StartExecutionAsync(int id)
    {
        var project = await GetProjectOrThrowAsync(id);
        project.StartExecution();
        await SaveChangesAsync(project);
    }

    public async Task CompleteProjectAsync(int id)
    {
        var project = await GetProjectOrThrowAsync(id);
        project.CompleteProject();
        await SaveChangesAsync(project);
    }

    public async Task CancelProjectAsync(int id)
    {
        var project = await GetProjectOrThrowAsync(id);
        project.CancelProject();
        await SaveChangesAsync(project);
    }

    private async Task<Project> GetProjectOrThrowAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
            throw new ApplicationValidationException($"No se encontró ningún proyecto con el ID {id}.");
        return project;
    }

    private async Task SaveChangesAsync(Project project)
    {
        await _projectRepository.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }
}