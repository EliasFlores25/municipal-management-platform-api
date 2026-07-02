using Application.Exceptions;
using Domain.Abstractions;
using Domain.Enums;
using MapsterMapper;
using static Application.DTOs.EmployeeDtos;

namespace Application.UseCases.Employees;

public class GetEmployeeQueriesUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeeQueriesUseCase(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<EmployeeResponse> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            throw new ApplicationValidationException($"No se encontró el registro del empleado con ID {id}.");
        }
        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeResponse> GetByCodeAsync(string code)
    {
        var normalizedCode = code?.Trim().ToUpper() ?? string.Empty;
        var employee = await _employeeRepository.GetByCodeAsync(normalizedCode);
        if (employee is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún empleado con el código empresarial: {normalizedCode}.");
        }
        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetByStatusAsync(EmployeeStatus status)
    {
        var employees = await _employeeRepository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetByPositionAsync(int positionId)
    {
        var employees = await _employeeRepository.GetByPositionAsync(positionId);
        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetByMunicipalityAsync(int municipalityId)
    {
        var employees = await _employeeRepository.GetByMunicipalityAsync(municipalityId);
        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }
    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()

    => _mapper.Map<IEnumerable<EmployeeResponse>>(
        await _employeeRepository.GetAllAsync());

}