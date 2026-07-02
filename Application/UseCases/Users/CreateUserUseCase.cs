using Application.Abstractions;
using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.UserDtos;

namespace Application.UseCases.Users;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserUseCase(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> ExecuteAsync(UserCreateRequest request)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            throw new ApplicationValidationException($"No se puede crear el usuario: El rol con ID {request.RoleId} no existe.");
        }

        var normalizedEmail = request.Email?.Trim().ToLower() ?? string.Empty;
        if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
        {
            throw new ApplicationValidationException($"El correo electrónico '{normalizedEmail}' ya está registrado.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ApplicationValidationException("La contraseña es obligatoria.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(request.Name, request.Email, passwordHash, request.RoleId);

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }
}