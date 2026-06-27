using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Notice
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime RegistrationDate { get; private set; }
    public NoticeCategory Category { get; private set; }
    public bool IsArchived { get; private set; }
    public int MunicipalityId { get; private set; }
    public Municipality? Municipality { get; private set; }

    protected Notice() { }

    public Notice(string title, string description, NoticeCategory category, int municipalityId, DateTime? registrationDate = null)
    {
        if (municipalityId <= 0)
            throw new DomainException("El ID del municipio debe ser mayor a cero.");

        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim().ToUpper();
        Description = description.Trim();
        Category = category;
        MunicipalityId = municipalityId;
        RegistrationDate = registrationDate ?? DateTime.UtcNow;
        IsArchived = false;
    }

    public void UpdateContent(string title, string description, NoticeCategory category)
    {
        if (IsArchived)
            throw new DomainException("No se puede modificar un aviso que ya ha sido archivado.");

        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim().ToUpper();
        Description = description.Trim();
        Category = category;
    }

    public void Archive()
    {
        if (IsArchived)
            throw new DomainException("El aviso ya se encuentra archivado.");

        IsArchived = true;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del aviso no puede estar vacío.");

        var trimmed = title.Trim();
        if (trimmed.Length is < 10 or > 50)
            throw new DomainException("El título del aviso debe tener entre 10 y 50 caracteres.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del aviso no puede estar vacía.");

        var trimmed = description.Trim();
        if (trimmed.Length is < 25 or > 200)
            throw new DomainException("La descripción del aviso debe tener entre 25 y 200 caracteres.");
    }
}