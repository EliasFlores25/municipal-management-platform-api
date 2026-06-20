using Application.Exceptions;
using Domain.Abstractions;
using static Application.DTOs.RoleDtos;

namespace Application.UseCase.Roles
{
    public class GetRoleByIdUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleByIdUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Response> ExecuteAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role is null)
            {
                throw new ApplicationValidationException($"No se encontró ningún rol con el ID {id}.");
            }

            return new Response(role.Id, role.Name);
        }
    }
}
