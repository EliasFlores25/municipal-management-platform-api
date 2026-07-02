using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.EmployeeDtos;

namespace Application.UseCases.Employees;

public class UpdateEmployeeProfileUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeProfileUseCase(
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

    public async Task ExecuteAsync(EmployeeUpdateProfileRequest request)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id);
        if (employee is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún empleado con el ID {request.Id}.");
        }

        if (employee.PositionId != request.PositionId)
        {
            var position = await _positionRepository.GetByIdAsync(request.PositionId);
            if (position is null)
                throw new ApplicationValidationException($"El cargo con ID {request.PositionId} no existe.");
        }

        if (employee.MunicipalityId != request.MunicipalityId)
        {
            var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
            if (municipality is null)
                throw new ApplicationValidationException($"El municipio con ID {request.MunicipalityId} no existe.");
        }

        employee.UpdateProfile(request.FirstName, request.LastName, request.PositionId, request.MunicipalityId);

        await _employeeRepository.UpdateAsync(employee);
        await _unitOfWork.SaveChangesAsync();
    }
}