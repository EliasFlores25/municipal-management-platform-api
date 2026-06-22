using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using static Application.DTOs.PositionDtos;

namespace Application.UseCases.Positions
{
    public class CreatePositionUseCase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePositionUseCase(IPositionRepository positionRepository, IUnitOfWork unitOfWork)
        {
            _positionRepository = positionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PositionResponse> ExecuteAsync(PositionCreateRequest request)
        {
            var normalizedName = request.Name?.Trim() ?? string.Empty;

            if (await _positionRepository.ExistsByNameAsync(normalizedName))
            {
                throw new ApplicationValidationException($"El cargo '{normalizedName}' ya se encuentra registrado.");
            }

            var position = new Position(request.Name, request.Description);

            await _positionRepository.AddAsync(position);
            await _unitOfWork.SaveChangesAsync();

            return new PositionResponse(position.Id, position.Name, position.Description);
        }
    }
}
