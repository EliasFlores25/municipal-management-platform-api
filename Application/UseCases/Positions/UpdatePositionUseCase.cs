using Domain.Abstractions;
using Application.Exceptions;
using static Application.DTOs.PositionDtos;

namespace Application.UseCases.Positions
{
    public class UpdatePositionUseCase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePositionUseCase(IPositionRepository positionRepository, IUnitOfWork unitOfWork)
        {
            _positionRepository = positionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(PositionUpdateRequest request)
        {
            var position = await _positionRepository.GetByIdAsync(request.Id);
            if (position is null)
            {
                throw new ApplicationValidationException($"No se encontró ningún cargo con el ID {request.Id}.");
            }

            var normalizedName = request.Name?.Trim() ?? string.Empty;

            if (position.Name != normalizedName && await _positionRepository.ExistsByNameAsync(normalizedName))
            {
                throw new ApplicationValidationException($"No se puede actualizar: El cargo '{normalizedName}' ya existe.");
            }

            position.UpdatePosition(request.Name, request.Description);

            await _positionRepository.UpdateAsync(position);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
