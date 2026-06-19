using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Project
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal Budget { get; private set; }
    public ProjectStatus State { get; private set; }
    public int MunicipalityId { get; private set; }
    public Municipality? Municipality { get; private set; }

    protected Project() { }

    public Project(string name, string description, DateTime endDate, decimal budget, int municipalityId, DateTime? startDate = null)
    {
        if (municipalityId <= 0)
            throw new DomainException("El ID del municipio debe ser mayor a cero.");

        if (budget < 0)
            throw new DomainException("El presupuesto del proyecto no puede ser negativo.");

        ValidateName(name);
        ValidateDescription(description);

        var finalStartDate = startDate ?? DateTime.UtcNow;
        if (endDate <= finalStartDate)
            throw new DomainException("La fecha de finalización debe ser posterior a la fecha de inicio.");

        Name = name.Trim();
        Description = description.Trim();
        StartDate = finalStartDate;
        EndDate = endDate;
        Budget = budget;
        State = ProjectStatus.Planificado;
        MunicipalityId = municipalityId;
    }

    public void UpdatePlanningDetails(string name, string description, DateTime endDate, decimal budget)
    {
        if (State is ProjectStatus.Finalizado or ProjectStatus.Cancelado)
            throw new DomainException($"No se puede modificar un proyecto que ya está en estado: {State}.");

        if (budget < 0)
            throw new DomainException("El presupuesto no puede ser negativo.");

        if (endDate <= StartDate)
            throw new DomainException("La nueva fecha de finalización debe ser posterior a la fecha de inicio.");

        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description.Trim();
        EndDate = endDate;
        Budget = budget;
    }

    public void StartExecution()
    {
        if (State != ProjectStatus.Planificado)
            throw new DomainException("Solo se pueden iniciar proyectos que se encuentren en estado Planificado.");

        State = ProjectStatus.EnEjecucion;
    }

    public void CompleteProject()
    {
        if (State != ProjectStatus.EnEjecucion)
            throw new DomainException("No se puede finalizar un proyecto que no esté actualmente En Ejecución.");

        State = ProjectStatus.Finalizado;
    }

    public void CancelProject()
    {
        if (State == ProjectStatus.Finalizado)
            throw new DomainException("No se puede cancelar un proyecto que ya fue completado exitosamente.");

        if (State == ProjectStatus.Cancelado)
            throw new DomainException("El proyecto ya se encuentra cancelado.");

        State = ProjectStatus.Cancelado;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del proyecto no puede estar vacío.");

        var trimmed = name.Trim();
        if (trimmed.Length is < 10 or > 50)
            throw new DomainException("El nombre del proyecto debe tener entre 10 y 50 caracteres.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del proyecto no puede estar vacía.");

        var trimmed = description.Trim();
        if (trimmed.Length is < 10 or > 200)
            throw new DomainException("La descripción del proyecto debe tener entre 10 y 200 caracteres.");
    }
}