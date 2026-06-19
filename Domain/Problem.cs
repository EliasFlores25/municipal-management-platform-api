using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Problem
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime RegistrationDate { get; private set; }
    public ProblemType Type { get; private set; }
    public ProblemSeverity Severity { get; private set; }
    public ProblemStatus Status { get; private set; }
    public int MunicipalityId { get; private set; }
    public Municipality? Municipality { get; private set; }

    protected Problem() { }

    public Problem(string title, string description, ProblemType type, ProblemSeverity severity, int municipalityId, DateTime? registrationDate = null)
    {
        if (municipalityId <= 0)
            throw new DomainException("El ID del municipio debe ser mayor a cero.");

        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim();
        Description = description.Trim();
        Type = type;
        Severity = severity;
        RegistrationDate = registrationDate ?? DateTime.UtcNow;
        Status = ProblemStatus.Reportado;
        MunicipalityId = municipalityId;
    }

    public void UpdateReport(string title, string description, ProblemType type, ProblemSeverity severity)
    {
        if (Status != ProblemStatus.Reportado)
            throw new DomainException("No se puede modificar un reporte que ya está en proceso o solucionado.");

        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim();
        Description = description.Trim();
        Type = type;
        Severity = severity;
    }

    public void MarkAsInProcess()
    {
        if (Status is ProblemStatus.Solucionado or ProblemStatus.Rechazado)
            throw new DomainException($"No se puede procesar un reporte que ya está en estado: {Status}.");

        Status = ProblemStatus.EnProceso;
    }

    public void Resolve()
    {
        if (Status == ProblemStatus.Solucionado)
            throw new DomainException("El reporte ya ha sido marcado como solucionado.");

        if (Status == ProblemStatus.Rechazado)
            throw new DomainException("No se puede solucionar un reporte que fue previamente rechazado.");

        Status = ProblemStatus.Solucionado;
    }

    public void Reject()
    {
        if (Status == ProblemStatus.Solucionado)
            throw new DomainException("No se puede rechazar un reporte que ya fue solucionado.");

        Status = ProblemStatus.Rechazado;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del problema no puede estar vacío.");

        var trimmed = title.Trim();
        if (trimmed.Length is < 10 or > 50)
            throw new DomainException("El título del problema debe tener entre 10 y 50 caracteres.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del problema no puede estar vacía.");

        var trimmed = description.Trim();
        if (trimmed.Length is < 10 or > 200)
            throw new DomainException("La descripción del problema debe tener entre 10 y 200 caracteres.");
    }
}