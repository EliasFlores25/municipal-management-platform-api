using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Inventory
{
    public int Id { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public DateTime EntryDate { get; private set; }
    public InventoryStatus State { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public int MunicipalityId { get; private set; }
    public Municipality? Municipality { get; private set; }

    protected Inventory() { }

    public Inventory(string itemName, string description, int quantity, int municipalityId, string imageUrl, DateTime? entryDate = null)
    {
        if (municipalityId <= 0) throw new DomainException("El ID del municipio debe ser mayor a cero.");
        if (quantity < 0) throw new DomainException("La cantidad inicial no puede ser negativa.");
        if (string.IsNullOrWhiteSpace(imageUrl)) throw new DomainException("La URL de la imagen es obligatoria.");

        ValidateItemName(itemName);
        ValidateDescription(description);

        ItemName = itemName.Trim();
        Description = description.Trim();
        Quantity = quantity;
        MunicipalityId = municipalityId;
        ImageUrl = imageUrl.Trim();
        EntryDate = entryDate ?? DateTime.UtcNow;

        State = quantity == 0 ? InventoryStatus.Agotado : InventoryStatus.Disponible;
    }

    public void UpdateBasicInfo(string description, string imageUrl)
    {
        if (State == InventoryStatus.Baja)
            throw new DomainException("No se puede modificar la información de un ítem que fue dado de baja.");

        if (string.IsNullOrWhiteSpace(imageUrl)) throw new DomainException("La URL de la imagen no puede estar vacía.");
        ValidateDescription(description);

        Description = description.Trim();
        ImageUrl = imageUrl.Trim();
    }

    public void IncreaseStock(int amount)
    {
        if (State == InventoryStatus.Baja)
            throw new DomainException("No se puede aumentar el stock de un ítem dado de baja.");

        if (amount <= 0)
            throw new DomainException("La cantidad a aumentar debe ser mayor a cero.");

        Quantity += amount;

        if (State == InventoryStatus.Agotado)
        {
            State = InventoryStatus.Disponible;
        }
    }

    public void DecreaseStock(int amount)
    {
        if (State == InventoryStatus.Baja)
            throw new DomainException("No se puede retirar stock de un ítem dado de baja.");

        if (amount <= 0)
            throw new DomainException("La cantidad a retirar debe ser mayor a cero.");

        if (Quantity - amount < 0)
            throw new DomainException($"Stock insuficiente. Intenta retirar {amount} pero solo hay {Quantity} disponibles.");

        Quantity -= amount;

        if (Quantity == 0)
        {
            State = InventoryStatus.Agotado;
        }
    }

    public void FlagAsBaja()
    {
        if (State == InventoryStatus.Baja)
            throw new DomainException("El ítem ya se encuentra dado de baja.");

        State = InventoryStatus.Baja;
        Quantity = 0;
    }

    private static void ValidateItemName(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) throw new DomainException("El nombre del ítem no puede estar vacío.");
        var trimmed = itemName.Trim();
        if (trimmed.Length is < 10 or > 50) throw new DomainException("El nombre del ítem debe tener entre 10 y 50 caracteres.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("La descripción no puede estar vacía.");
        var trimmed = description.Trim();
        if (trimmed.Length is < 10 or > 100) throw new DomainException("La descripción debe tener entre 10 y 100 caracteres.");
    }
}