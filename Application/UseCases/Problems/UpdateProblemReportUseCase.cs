using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.ProblemDtos;

namespace Application.UseCases.Problems;

public class UpdateProblemReportUseCase
{
    private readonly IProblemRepository _problemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProblemReportUseCase(IProblemRepository problemRepository, IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ProblemUpdateReportRequest request)
    {
        var problem = await _problemRepository.GetByIdAsync(request.Id);
        if (problem is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún reporte con el ID {request.Id}.");
        }

        // El dominio valida que siga en estado "Reportado" antes de permitir la edición
        problem.UpdateReport(request.Title, request.Description, request.Type, request.Severity);

        await _problemRepository.UpdateAsync(problem);
        await _unitOfWork.SaveChangesAsync();
    }
}