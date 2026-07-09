using Domain.Exceptions;

namespace Domain;

public class Position
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    protected Position() { }

    public Position(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description.Trim();
    }

    public void UpdatePosition(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = description.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del cargo no puede estar vacío.");

        var trimmed = name.Trim();

        if (trimmed.Length is < 4 or > 50)
            throw new DomainException("El nombre del cargo debe tener entre 4 y 50 caracteres.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del cargo no puede estar vacía.");

        var trimmed = description.Trim();
        if (trimmed.Length is < 25 or > 200)
            throw new DomainException("La descripción del cargo debe tener entre 25 y 200 caracteres.");
    }
}