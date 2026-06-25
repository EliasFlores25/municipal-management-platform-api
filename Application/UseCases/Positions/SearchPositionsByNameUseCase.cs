using Domain.Abstractions;
using static Application.DTOs.PositionDtos;

namespace Application.UseCases.Positions
{
    public class SearchPositionsByNameUseCase
    {
        private readonly IPositionRepository _positionRepository;

        public SearchPositionsByNameUseCase(IPositionRepository positionRepository)
            => _positionRepository = positionRepository;

        public async Task<IEnumerable<PositionResponse>> ExecuteAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<PositionResponse>();
            }

            var positions = await _positionRepository.SearchByNameAsync(searchTerm.Trim());
            return positions.Select(p => new PositionResponse(p.Id, p.Name, p.Description));
        }
    }
}
