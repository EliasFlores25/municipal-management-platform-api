using Application.Exceptions;
using Domain.Abstractions;
using Domain;
using static Application.DTOs.EmployeeDtos;

namespace Application.UseCases.Employees;

public class EmployeeLifecycleUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeLifecycleUseCase(
        IEmployeeRepository employeeRepository,
        IPositionRepository positionRepository,
        IMunicipalityRepository municipalityRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _positionRepository = positionRepository;
        _municipalityRepository = municipalityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task TerminateAsync(EmployeeTerminateRequest request)
    {
        var employee = await GetEmployeeOrThrowAsync(request.Id);

        employee.TerminateEmployment(request.ExitDate);

        await SaveChangesAsync(employee);
    }

    public async Task ReactivateAsync(EmployeeReactivateRequest request)
    {
        var employee = await GetEmployeeOrThrowAsync(request.Id);

        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        if (position is null)
            throw new ApplicationValidationException($"El cargo con ID {request.PositionId} no existe.");

        var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
        if (municipality is null)
            throw new ApplicationValidationException($"El municipio con ID {request.MunicipalityId} no existe.");

        employee.ReactivateEmployment(request.PositionId, request.MunicipalityId, request.NewContractDate);

        await SaveChangesAsync(employee);
    }

    private async Task<Domain.Employee> GetEmployeeOrThrowAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
            throw new ApplicationValidationException($"No se encontró el registro laboral del empleado con ID {id}.");
        return employee;
    }

    private async Task SaveChangesAsync(Employee employee)
    {
        await _employeeRepository.UpdateAsync(employee);
        await _unitOfWork.SaveChangesAsync();
    }
}