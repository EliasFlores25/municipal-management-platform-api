using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.UserDtos;

namespace Application.UseCases.Users;

public class UpdateUserProfileUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileUseCase(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(UserUpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún usuario con el ID {request.Id}.");
        }

        if (user.RoleId != request.RoleId)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role is null)
            {
                throw new ApplicationValidationException($"El rol con ID {request.RoleId} no existe.");
            }
        }

        var normalizedEmail = request.Email?.Trim().ToLower() ?? string.Empty;
        if (user.Email != normalizedEmail && await _userRepository.ExistsByEmailAsync(normalizedEmail))
        {
            throw new ApplicationValidationException($"El correo electrónico '{normalizedEmail}' ya pertenece a otra cuenta.");
        }

        user.UpdateProfile(request.Name, request.Email, request.RoleId);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}