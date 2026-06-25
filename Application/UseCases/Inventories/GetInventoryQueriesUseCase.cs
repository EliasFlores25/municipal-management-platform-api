using Application.Exceptions;
using Domain.Abstractions;
using Domain.Enums;
using MapsterMapper;
using static Application.DTOs.InventoryDtos;

namespace Application.UseCases.Inventories;

public class GetInventoryQueriesUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoryQueriesUseCase(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<InventoryResponse> GetByIdAsync(int id)
    {
        var item = await _inventoryRepository.GetByIdAsync(id);

        if (item is null)
            throw new ApplicationValidationException($"No existe el ítem con ID {id}.");

        return _mapper.Map<InventoryResponse>(item);
    }

    public async Task<IEnumerable<InventoryResponse>> GetByMunicipalityAsync(int municipalityId)
    {
        var items = await _inventoryRepository.GetByMunicipalityAsync(municipalityId);
        return _mapper.Map<IEnumerable<InventoryResponse>>(items);
    }

    public async Task<IEnumerable<InventoryResponse>> GetByStatusAsync(InventoryStatus status)
    {
        var items = await _inventoryRepository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<InventoryResponse>>(items);
    }

    public async Task<IEnumerable<InventoryResponse>> GetLowStockAsync(int threshold)
    {
        var items = await _inventoryRepository.GetLowStockItemsAsync(threshold);
        return _mapper.Map<IEnumerable<InventoryResponse>>(items);
    }

    public async Task<IEnumerable<InventoryResponse>> GetAllOrderedAsync()

        => _mapper.Map<IEnumerable<InventoryResponse>>(
            await _inventoryRepository.GetAllOrderedByItemNameAsync());
}