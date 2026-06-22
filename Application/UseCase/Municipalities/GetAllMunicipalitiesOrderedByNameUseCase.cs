using Domain.Abstractions;
using static Application.DTOs.MunicipalityDtos;

namespace Application.UseCases.Municipalities
{
    public class GetAllMunicipalitiesOrderedByNameUseCase
    {
        private readonly IMunicipalityRepository _municipalityRepository;

        public GetAllMunicipalitiesOrderedByNameUseCase(IMunicipalityRepository municipalityRepository)
        {
            _municipalityRepository = municipalityRepository;
        }

        public async Task<IEnumerable<MunicipalityResponse>> ExecuteAsync()
        {
            var municipalities = await _municipalityRepository.GetAllOrderedByNameAsync();

            return municipalities.Select(m => new MunicipalityResponse(m.Id, m.Name));
        }
    }
}