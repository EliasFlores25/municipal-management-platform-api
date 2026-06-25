using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.InventoryDtos;

namespace Application.UseCases.Inventories
{
    public class CreateInventoryUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMunicipalityRepository _municipalityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateInventoryUseCase(
            IInventoryRepository inventoryRepository,
            IMunicipalityRepository municipalityRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _municipalityRepository = municipalityRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<InventoryResponse> ExecuteAsync(InventoryCreateRequest request)
        {
            var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);

            if (municipality is null)
                throw new ApplicationValidationException($"No se puede crear el ítem: El municipio con ID {request.MunicipalityId} no existe.");

            var trimmedName = request.ItemName?.Trim() ?? string.Empty;

            if (await _inventoryRepository.ExistsByNameInMunicipalityAsync(trimmedName, request.MunicipalityId))
                throw new ApplicationValidationException($"El ítem '{trimmedName}' ya está registrado en este municipio.");

            var inventory = new Inventory(
                request.ItemName,
                request.Description,
                request.Quantity,
                request.MunicipalityId,
                request.ImageUrl);

            await _inventoryRepository.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InventoryResponse>(inventory);
        }
    }
}
