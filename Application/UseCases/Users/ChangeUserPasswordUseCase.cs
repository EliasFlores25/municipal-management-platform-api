using Application.Abstractions;
using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.UserDtos;

namespace Application.UseCases.Users;

public class ChangeUserPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeUserPasswordUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(UserChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún usuario con el ID {request.Id}.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new ApplicationValidationException("La nueva contraseña no puede estar vacía.");
        }

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.ChangePassword(newHash);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}