using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.MunicipalityDtos;

namespace Application.UseCases.Municipalities
{
    public class UpdateMunicipalityUseCase
    {
        private readonly IMunicipalityRepository _municipalityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMunicipalityUseCase(IMunicipalityRepository municipalityRepository, IUnitOfWork unitOfWork)
        {
            _municipalityRepository = municipalityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(MunicipalityUpdateRequest request)
        {
            var municipality = await _municipalityRepository.GetByIdAsync(request.Id);
            if (municipality is null)
            {
                throw new ApplicationValidationException($"No se encontró ningún municipio con el ID {request.Id}.");
            }

            var normalizedName = request.Name?.Trim().ToUpper() ?? string.Empty;

            if (municipality.Name != normalizedName && await _municipalityRepository.ExistsByNameAsync(normalizedName))
            {
                throw new ApplicationValidationException($"No se puede actualizar: El municipio '{normalizedName}' ya existe.");
            }

            municipality.UpdateMunicipality(request.Name);

            await _municipalityRepository.UpdateAsync(municipality);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}