using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.Positions
{
    public class DeletePositionUseCase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePositionUseCase(IPositionRepository positionRepository, IUnitOfWork unitOfWork)
        {
            _positionRepository = positionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int id)
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if (position is null)
            {
                throw new ApplicationValidationException($"No se puede eliminar: No existe el cargo con ID {id}.");
            }

            await _positionRepository.DeleteAsync(position);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
