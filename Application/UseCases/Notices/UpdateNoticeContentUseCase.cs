using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.NoticeDtos;

namespace Application.UseCases.Notices;

public class UpdateNoticeContentUseCase
{
    private readonly INoticeRepository _noticeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNoticeContentUseCase(INoticeRepository noticeRepository, IUnitOfWork unitOfWork)
    {
        _noticeRepository = noticeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(NoticeUpdateContentRequest request)
    {
        var notice = await _noticeRepository.GetByIdAsync(request.Id);
        if (notice is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún aviso con el ID {request.Id}.");
        }

        notice.UpdateContent(request.Title, request.Description, request.Category);

        await _noticeRepository.UpdateAsync(notice);
        await _unitOfWork.SaveChangesAsync();
    }
}