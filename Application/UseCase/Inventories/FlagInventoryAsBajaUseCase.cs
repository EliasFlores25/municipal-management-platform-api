using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.Inventories;

public class FlagInventoryAsBajaUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FlagInventoryAsBajaUseCase(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory is null)
        {
            throw new ApplicationValidationException($"No se encontró el ítem con ID {id}.");
        }

        inventory.FlagAsBaja();

        await _inventoryRepository.UpdateAsync(inventory);
        await _unitOfWork.SaveChangesAsync();
    }
}