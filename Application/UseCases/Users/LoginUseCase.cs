using Application.Abstractions;
using Application.Exceptions;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.UserDtos;

namespace Application.UseCases.Users;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email?.Trim().ToLower() ?? string.Empty;
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null || !user.IsActive)
        {
            throw new ApplicationValidationException("Las credenciales proporcionadas no son válidas.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new ApplicationValidationException("Las credenciales proporcionadas no son válidas.");
        }

        var token = _tokenService.GenerateJwtToken(user);

        var userResponse = _mapper.Map<UserResponse>(user);

        return new LoginResponse(token, userResponse);
    }
}