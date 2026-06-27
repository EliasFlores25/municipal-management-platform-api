using Application.Exceptions;
using Domain.Abstractions;
using Domain.Enums;
using MapsterMapper;
using static Application.DTOs.NoticeDtos;

namespace Application.UseCases.Notices;

public class GetNoticeQueriesUseCase
{
    private readonly INoticeRepository _noticeRepository;
    private readonly IMapper _mapper;

    public GetNoticeQueriesUseCase(INoticeRepository noticeRepository, IMapper mapper)
    {
        _noticeRepository = noticeRepository;
        _mapper = mapper;
    }

    public async Task<NoticeResponse> GetByIdAsync(int id)
    {
        var notice = await _noticeRepository.GetByIdAsync(id);
        if (notice is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún aviso con el ID {id}.");
        }
        return _mapper.Map<NoticeResponse>(notice);
    }
    public async Task<IEnumerable<NoticeResponse>> GetActiveByMunicipalityAsync(int municipalityId)
    {
        var notices = await _noticeRepository.GetActiveByMunicipalityAsync(municipalityId);
        return _mapper.Map<IEnumerable<NoticeResponse>>(notices);
    }

    public async Task<IEnumerable<NoticeResponse>> GetByCategoryAsync(NoticeCategory category)
    {
        var notices = await _noticeRepository.GetByCategoryAsync(category);
        return _mapper.Map<IEnumerable<NoticeResponse>>(notices);
    }
    public async Task<IEnumerable<NoticeResponse>> GetActiveByCategoryAsync(NoticeCategory category)
    {
        var notices = await _noticeRepository.GetActiveByCategoryAsync(category);
        return _mapper.Map<IEnumerable<NoticeResponse>>(notices);
    }
    public async Task<IEnumerable<NoticeResponse>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ApplicationValidationException("La fecha final del rango de consulta no puede ser menor a la fecha inicial.");
        }

        var notices = await _noticeRepository.GetByRegistrationDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<NoticeResponse>>(notices);
    }
    public async Task<IEnumerable<NoticeResponse>> GetAllAsync()

        => _mapper.Map<IEnumerable<NoticeResponse>>(
            await _noticeRepository.GetAllAsync());
}