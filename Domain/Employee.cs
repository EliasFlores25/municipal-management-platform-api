using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Employee
{
    public int Id { get; private set; } 
    public string Code { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime ContractDate { get; private set; }
    public DateTime? ExitDate { get; private set; }
    public EmployeeStatus State { get; private set; }
    public int PositionId { get; private set; }
    public Position? Position { get; private set; }
    public int MunicipalityId { get; private set; }
    public Municipality? Municipality { get; private set; }

    public TimeSpan? ContractDuration => ExitDate.HasValue ? ExitDate.Value - ContractDate : null;
    public string FullName => $"{FirstName} {LastName}";

    protected Employee() { }

    public Employee(string code, string firstName, string lastName, int positionId, int municipalityId, DateTime? contractDate = null)
    {
        if (positionId <= 0) throw new DomainException("El ID del cargo debe ser mayor a cero.");
        if (municipalityId <= 0) throw new DomainException("El ID del municipio debe ser mayor a cero.");

        ValidateCode(code);
        ValidateFirstName(firstName);
        ValidateLastName(lastName);

        Code = code.Trim().ToUpper();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PositionId = positionId;
        MunicipalityId = municipalityId;

        State = EmployeeStatus.Activo;
        ContractDate = contractDate ?? DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, int positionId, int municipalityId)
    {
        if (positionId <= 0) throw new DomainException("El ID del cargo debe ser mayor a cero.");
        if (municipalityId <= 0) throw new DomainException("El ID del municipio debe ser mayor a cero.");
        if (State == EmployeeStatus.Inactivo) throw new DomainException("No se pueden modificar los datos de un empleado inactivo.");

        ValidateFirstName(firstName);
        ValidateLastName(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PositionId = positionId;
        MunicipalityId = municipalityId;
    }

    public void TerminateEmployment(DateTime? exitDate = null)
    {
        if (State == EmployeeStatus.Inactivo)
            throw new DomainException("El empleado ya se encuentra inactivo.");

        var finalExitDate = exitDate ?? DateTime.UtcNow;

        if (finalExitDate < ContractDate)
            throw new DomainException("La fecha de salida no puede ser anterior a la fecha de contratación.");

        State = EmployeeStatus.Inactivo;
        ExitDate = finalExitDate;
    }

    public void ReactivateEmployment(int positionId, int municipalityId, DateTime? newContractDate = null)
    {
        if (State == EmployeeStatus.Activo)
            throw new DomainException("El empleado ya está activo.");

        State = EmployeeStatus.Activo;
        ContractDate = newContractDate ?? DateTime.UtcNow;
        ExitDate = null;
        PositionId = positionId;
        MunicipalityId = municipalityId;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("El código no puede estar vacío.");
        var trimmed = code.Trim();
        if (trimmed.Length is < 3 or > 20) throw new DomainException("El código debe tener entre 3 y 20 caracteres.");
    }

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainException("El nombre no puede estar vacío.");
        var trimmed = firstName.Trim();
        if (trimmed.Length is < 2 or > 50) throw new DomainException("El nombre debe tener entre 2 y 50 caracteres.");
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainException("El apellido no puede estar vacío.");
        var trimmed = lastName.Trim();
        if (trimmed.Length is < 2 or > 50) throw new DomainException("El apellido debe tener entre 2 y 50 caracteres.");
    }
}