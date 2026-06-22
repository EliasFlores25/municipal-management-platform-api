using Domain.Abstractions;
using static Application.DTOs.PositionDtos;

namespace Application.UseCases.Positions
{
    public class GetAllPositionsOrderedByNameUseCase
    {
        private readonly IPositionRepository _positionRepository;

        public GetAllPositionsOrderedByNameUseCase(IPositionRepository positionRepository)
            => _positionRepository = positionRepository;

        public async Task<IEnumerable<PositionResponse>> ExecuteAsync()
        {
            var positions = await _positionRepository.GetAllOrderedByNameAsync();
            return positions.Select(p => new PositionResponse(p.Id, p.Name, p.Description));
        }
    }
}
