using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.Municipalities
{
    public class DeleteMunicipalityUseCase
    {
        private readonly IMunicipalityRepository _municipalityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMunicipalityUseCase(IMunicipalityRepository municipalityRepository, IUnitOfWork unitOfWork)
        {
            _municipalityRepository = municipalityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int id)
        {
            var municipality = await _municipalityRepository.GetByIdAsync(id);
            if (municipality is null)
            {
                throw new ApplicationValidationException($"No se puede eliminar: No existe el municipio con ID {id}.");
            }

            await _municipalityRepository.DeleteAsync(municipality);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}