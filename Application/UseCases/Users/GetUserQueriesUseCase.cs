using Application.Exceptions;
using Domain.Abstractions;
using MapsterMapper;
using static Application.DTOs.UserDtos;

namespace Application.UseCases.Users;

public class GetUserQueriesUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueriesUseCase(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            throw new ApplicationValidationException($"No existe ningún usuario registrado con el ID {id}.");
        }
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetByEmailAsync(string email)
    {
        var normalizedEmail = email?.Trim().ToLower() ?? string.Empty;
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (user is null)
        {
            throw new ApplicationValidationException($"No se encontró ningún usuario con el correo: {normalizedEmail}.");
        }
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<IEnumerable<UserResponse>> GetByRoleAsync(int roleId)
    {
        var users = await _userRepository.GetByRoleAsync(roleId);
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<IEnumerable<UserResponse>> GetByStatusAsync(bool isActive)
    {
        var users = await _userRepository.GetByStatusAsync(isActive);
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }
}