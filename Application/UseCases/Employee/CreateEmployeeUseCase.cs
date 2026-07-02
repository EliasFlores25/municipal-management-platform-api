using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.EmployeeDtos;

namespace Application.UseCases.Employees;

public class CreateEmployeeUseCase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeUseCase(
        IEmployeeRepository employeeRepository,
        IPositionRepository positionRepository,
        IMunicipalityRepository municipalityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _positionRepository = positionRepository;
        _municipalityRepository = municipalityRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmployeeResponse> ExecuteAsync(EmployeeCreateRequest request)
    {
        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        if (position is null)
        {
            throw new ApplicationValidationException($"No se puede registrar al empleado: El cargo con ID {request.PositionId} no existe.");
        }

        var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
        if (municipality is null)
        {
            throw new ApplicationValidationException($"No se puede registrar al empleado: El municipio con ID {request.MunicipalityId} no existe.");
        }

        var normalizedCode = request.Code?.Trim().ToUpper() ?? string.Empty;
        if (await _employeeRepository.ExistsByCodeAsync(normalizedCode))
        {
            throw new ApplicationValidationException($"El código de empleado '{normalizedCode}' ya se encuentra asignado a otro trabajador.");
        }

        var employee = new Employee(
            request.Code,
            request.FirstName,
            request.LastName,
            request.PositionId,
            request.MunicipalityId);

        await _employeeRepository.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmployeeResponse>(employee);
    }
}