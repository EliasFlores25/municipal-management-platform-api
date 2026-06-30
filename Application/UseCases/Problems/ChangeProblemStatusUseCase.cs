using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.Problems;

public class ChangeProblemStatusUseCase
{
    private readonly IProblemRepository _problemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeProblemStatusUseCase(IProblemRepository problemRepository, IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task MarkAsInProcessAsync(int id)
    {
        var problem = await GetProblemOrThrowAsync(id);
        problem.MarkAsInProcess();
        await SaveChangesAsync(problem);
    }

    public async Task ResolveAsync(int id)
    {
        var problem = await GetProblemOrThrowAsync(id);
        problem.Resolve();
        await SaveChangesAsync(problem);
    }

    public async Task RejectAsync(int id)
    {
        var problem = await GetProblemOrThrowAsync(id);
        problem.Reject();
        await SaveChangesAsync(problem);
    }

    private async Task<Domain.Problem> GetProblemOrThrowAsync(int id)
    {
        var problem = await _problemRepository.GetByIdAsync(id);
        if (problem is null)
            throw new ApplicationValidationException($"No se encontró ningún reporte de problema con el ID {id}.");
        return problem;
    }

    private async Task SaveChangesAsync(Domain.Problem problem)
    {
        await _problemRepository.UpdateAsync(problem);
        await _unitOfWork.SaveChangesAsync();
    }
}