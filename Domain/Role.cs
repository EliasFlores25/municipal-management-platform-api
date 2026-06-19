using Domain.Exceptions;

namespace Domain;

public class Role
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    protected Role() { }

    public Role(string name)
    {
        ValidateName(name);
        Name = name.Trim().ToUpper();
    }
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del rol no puede estar vacío.");

        var trimmed = name.Trim();

        if (trimmed.Length is < 3 or > 30)
            throw new DomainException("El nombre del rol debe tener entre 3 y 30 caracteres.");
    }
}