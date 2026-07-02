using Domain.Abstractions;
using Application.Exceptions;
using Domain;

namespace Application.UseCases.Users;

public class UserStatusManagementUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserStatusManagementUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SuspendAsync(int id)
    {
        var user = await GetUserOrThrowAsync(id);
        user.Suspend();
        await SaveChangesAsync(user);
    }

    public async Task ReactivateAsync(int id)
    {
        var user = await GetUserOrThrowAsync(id);
        user.Reactivate();
        await SaveChangesAsync(user);
    }

    private async Task<User> GetUserOrThrowAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            throw new ApplicationValidationException($"No se encontró el usuario con el ID {id}.");
        return user;
    }

    private async Task SaveChangesAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}