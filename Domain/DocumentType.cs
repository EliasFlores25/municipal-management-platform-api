using Domain.Exceptions;

namespace Domain;

public class DocumentType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    protected DocumentType() { }

    public DocumentType(string name)
    {
        ValidateName(name);
        Name = name.Trim();
    }

    public void UpdateDocumentType(string name)
    {
        ValidateName(name);
        Name = name.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del tipo de documento no puede estar vacío.");

        var trimmed = name.Trim();
        if (trimmed.Length is < 5 or > 50)
            throw new DomainException("El nombre del tipo de documento debe tener entre 5 y 50 caracteres.");
    }
}