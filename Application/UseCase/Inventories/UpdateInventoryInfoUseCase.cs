using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.InventoryDtos;

namespace Application.UseCases.Inventories;

public class UpdateInventoryInfoUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryInfoUseCase(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(InventoryUpdateInfoRequest request)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id);
        if (inventory is null)
        {
            throw new ApplicationValidationException($"No se encontró el ítem de inventario con ID {request.Id}.");
        }

        inventory.UpdateBasicInfo(request.Description, request.ImageUrl);

        await _inventoryRepository.UpdateAsync(inventory);
        await _unitOfWork.SaveChangesAsync();
    }
}