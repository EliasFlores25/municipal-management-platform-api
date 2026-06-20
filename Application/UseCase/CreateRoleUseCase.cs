using Application.Exceptions;
using Domain;
using Domain.Abstractions;
using static Application.DTOs.RoleDtos;

namespace Application.UseCase
{
    public class CreateRoleUseCase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleUseCase(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> ExecuteAsync(CreateRequest request)
        {
            var normalizedName = request.Name?.Trim().ToUpper() ?? string.Empty;

            if (await _roleRepository.ExistsByRoleNameAsync(normalizedName))
            {
                throw new ApplicationValidationException($"El rol '{normalizedName}' ya existe en el sistema.");
            }
            var newRole = new Role(request.Name);

            await _roleRepository.AddAsync(newRole);
            await _unitOfWork.SaveChangesAsync();

            return new Response(newRole.Id, newRole.Name);
        }
    }
}
