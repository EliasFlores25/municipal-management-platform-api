using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.InventoryDtos;

namespace Application.UseCases.Inventories;

public class IncreaseInventoryStockUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncreaseInventoryStockUseCase(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(InventoryStockAdjustmentRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id);
        if (inventory is null)
        {
            throw new ApplicationValidationException($"No se encontró el ítem con ID {request.Id}.");
        }

        inventory.IncreaseStock(request.Amount);

        await _inventoryRepository.UpdateAsync(inventory);
        await _unitOfWork.SaveChangesAsync();
    }
}