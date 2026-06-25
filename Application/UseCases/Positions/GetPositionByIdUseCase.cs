using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.PositionDtos;

namespace Application.UseCases.Positions
{
    public class GetPositionByIdUseCase
    {
        private readonly IPositionRepository _positionRepository;

        public GetPositionByIdUseCase(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public async Task<PositionResponse> ExecuteAsync(int id)
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if (position is null)
            {
                throw new ApplicationValidationException($"No se encontró el cargo con ID {id}.");
            }

            return new PositionResponse(position.Id, position.Name, position.Description);
        }
    }
}
