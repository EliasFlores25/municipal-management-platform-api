using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using static Application.DTOs.MunicipalityDtos;

namespace Application.UseCases.Municipalities;

public class CreateMunicipalityUseCase
{
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMunicipalityUseCase(IMunicipalityRepository municipalityRepository, IUnitOfWork unitOfWork)
    {
        _municipalityRepository = municipalityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MunicipalityResponse> ExecuteAsync(MunicipalityCreateRequest request)
    {
        var normalizedName = request.Name?.Trim().ToUpper() ?? string.Empty;

        if (await _municipalityRepository.ExistsByNameAsync(normalizedName))
        {
            throw new ApplicationValidationException($"El municipio '{normalizedName}' ya se encuentra registrado.");
        }

        var municipality = new Municipality(request.Name);

        await _municipalityRepository.AddAsync(municipality);
        await _unitOfWork.SaveChangesAsync();

        return new MunicipalityResponse(municipality.Id, municipality.Name);
    }
}