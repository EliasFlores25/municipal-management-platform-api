using Domain.Abstractions;
using Application.Exceptions;

namespace Application.UseCases.Notices;

public class ArchiveNoticeUseCase
{
    private readonly INoticeRepository _noticeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveNoticeUseCase(INoticeRepository noticeRepository, IUnitOfWork unitOfWork)
    {
        _noticeRepository = noticeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id)
    {
        var notice = await _noticeRepository.GetByIdAsync(id);
        if (notice is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún aviso con el ID {id}.");
        }

        notice.Archive();

        await _noticeRepository.UpdateAsync(notice);
        await _unitOfWork.SaveChangesAsync();
    }
}