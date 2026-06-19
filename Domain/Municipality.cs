using Domain.Exceptions;

namespace Domain;

public class Municipality
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    protected Municipality() { }

    public Municipality(string name)
    {
        ValidateName(name);
        Name = name.Trim().ToUpper();
    }

    public void UpdateMunicipality(string name)
    {
        ValidateName(name);
        Name = name.Trim().ToUpper();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del municipio no puede estar vacío.");

        var trimmed = name.Trim();

        if (trimmed.Length is < 4 or > 50)
            throw new DomainException("El nombre del municipio debe tener entre 4 y 50 caracteres.");
    }
}