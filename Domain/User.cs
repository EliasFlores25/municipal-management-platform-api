using System.Text.RegularExpressions;
using Domain.Exceptions;

namespace Domain;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int RoleId { get; private set; }
    public Role? Role { get; private set; }

    protected User() { }

    public User(string name, string email, string passwordHash, int roleId)
    {
        if (roleId <= 0) throw new DomainException("El ID del rol debe ser mayor a cero.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("El hash de la contraseña es obligatorio.");

        ValidateName(name);
        ValidateEmail(email);

        Name = name.Trim();
        Email = email.Trim().ToLower();
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsActive = true;
    }

    public void UpdateProfile(string name, string email, int roleId)
    {
        if (!IsActive) throw new DomainException("No se puede modificar un usuario suspendido.");
        if (roleId <= 0) throw new DomainException("El ID del rol debe ser mayor a cero.");

        ValidateName(name);
        ValidateEmail(email);

        Name = name.Trim();
        Email = email.Trim().ToLower();
        RoleId = roleId;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (!IsActive) throw new DomainException("No se puede cambiar la contraseña de un usuario suspendido.");
        if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new DomainException("El nuevo hash de la contraseña no puede estar vacío.");

        PasswordHash = newPasswordHash;
    }

    public void Suspend()
    {
        if (!IsActive) throw new DomainException("La cuenta de usuario ya se encuentra suspendida.");
        IsActive = false;
    }

    public void Reactivate()
    {
        if (IsActive) throw new DomainException("La cuenta de usuario ya se encuentra activa.");
        IsActive = true;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("El nombre de usuario no puede estar vacío.");
        var trimmed = name.Trim();
        if (trimmed.Length is < 3 or > 50) throw new DomainException("El nombre de usuario debe tener entre 3 y 50 caracteres.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("El correo electrónico no puede estar vacío.");

        var trimmed = email.Trim();
        if (trimmed.Length > 100) throw new DomainException("El correo electrónico no puede exceder los 100 caracteres.");

        
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(trimmed, emailPattern, RegexOptions.Compiled)) 
            throw new DomainException("El formato del correo electrónico no es válido.");
    }
}