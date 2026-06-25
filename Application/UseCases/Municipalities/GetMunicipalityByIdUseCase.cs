using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.MunicipalityDtos;

namespace Application.UseCases.Municipalities;

public class GetMunicipalityByIdUseCase
{
    private readonly IMunicipalityRepository _municipalityRepository;

    public GetMunicipalityByIdUseCase(IMunicipalityRepository municipalityRepository)
    {
        _municipalityRepository = municipalityRepository;
    }

    public async Task<MunicipalityResponse> ExecuteAsync(int id)
    {
        var municipality = await _municipalityRepository.GetByIdAsync(id);
        if (municipality is null)
        {
            throw new ApplicationValidationException($"No se encontró el municipio con ID {id}.");
        }

        return new MunicipalityResponse(municipality.Id, municipality.Name);
    }
}