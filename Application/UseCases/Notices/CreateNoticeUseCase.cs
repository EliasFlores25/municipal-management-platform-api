using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.NoticeDtos;

namespace Application.UseCases.Notices;

public class CreateNoticeUseCase
{
    private readonly INoticeRepository _noticeRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNoticeUseCase(
        INoticeRepository noticeRepository,
        IMunicipalityRepository municipalityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _noticeRepository = noticeRepository;
        _municipalityRepository = municipalityRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<NoticeResponse> ExecuteAsync(NoticeCreateRequest request)
    {
        var municipality = await _municipalityRepository.GetByIdAsync(request.MunicipalityId);
        if (municipality is null)
        {
            throw new ApplicationValidationException($"No se puede registrar el aviso: El municipio con ID {request.MunicipalityId} no existe.");
        }

        var notice = new Notice(
            request.Title,
            request.Description,
            request.Category,
            request.MunicipalityId);

        await _noticeRepository.AddAsync(notice);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NoticeResponse>(notice);
    }
}