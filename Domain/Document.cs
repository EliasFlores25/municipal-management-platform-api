using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Document
{
    public int Id { get; private set; }
    public string DocumentNumber { get; private set; } = string.Empty;
    public DateTime EmissionDate { get; private set; }
    public string Proprietary { get; private set; } = string.Empty;
    public string Details { get; private set; } = string.Empty;
    public DocumentStatus State { get; private set; }
    public int DocumentTypeId { get; private set; }
    public DocumentType? DocumentType { get; private set; }
    public int MunicipalityId { get; private set; }
    public Municipality? Municipality { get; private set; }

    protected Document() { }

    public Document(string documentNumber, string proprietary, string details, int documentTypeId, int municipalityId, DateTime? emissionDate = null)
    {
        ValidateDocumentNumber(documentNumber);
        ValidateProprietary(proprietary);
        ValidateDetails(details);

        if (documentTypeId <= 0)
            throw new DomainException("El ID del tipo de documento debe ser mayor a cero.");

        if (municipalityId <= 0)
            throw new DomainException("El ID del municipio debe ser mayor a cero.");

        var finalEmissionDate = emissionDate ?? DateTime.UtcNow;
        if (finalEmissionDate > DateTime.UtcNow)
            throw new DomainException("La fecha de emisión no puede ser una fecha futura.");

        DocumentNumber = documentNumber.Trim().ToUpper();
        EmissionDate = finalEmissionDate;
        Proprietary = proprietary.Trim();
        Details = details.Trim();
        State = DocumentStatus.Vigente;
        DocumentTypeId = documentTypeId;
        MunicipalityId = municipalityId;
    }

    public void UpdateDetails(string proprietary, string details)
    {
        if (State == DocumentStatus.Anulado)
            throw new DomainException("No se puede modificar un documento que ya está anulado.");

        ValidateProprietary(proprietary);
        ValidateDetails(details);

        Proprietary = proprietary.Trim();
        Details = details.Trim();
    }

    public void Anular()
    {
        if (State == DocumentStatus.Anulado)
            throw new DomainException("El documento ya se encuentra anulado.");

        State = DocumentStatus.Anulado;
    }

    private static void ValidateDocumentNumber(string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new DomainException("El número de documento no puede estar vacío.");

        var trimmed = documentNumber.Trim();
        if (trimmed.Length is < 3 or > 20)
            throw new DomainException("El número de documento debe tener entre 3 y 20 caracteres.");
    }

    private static void ValidateProprietary(string proprietary)
    {
        if (string.IsNullOrWhiteSpace(proprietary))
            throw new DomainException("El nombre del propietario no puede estar vacío.");

        var trimmed = proprietary.Trim();
        if (trimmed.Length is < 2 or > 50)
            throw new DomainException("El nombre del propietario debe tener entre 2 y 50 caracteres.");
    }

    private static void ValidateDetails(string details)
    {
        if (string.IsNullOrWhiteSpace(details))
            throw new DomainException("El detalle del documento no puede estar vacío.");

        var trimmed = details.Trim();
        if (trimmed.Length is < 10 or > 100)
            throw new DomainException("El detalle del documento debe tener entre 10 y 100 caracteres.");
    }
}