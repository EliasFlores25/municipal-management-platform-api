using Domain.Abstractions;
using static Application.DTOs.RoleDtos;

namespace Application.UseCase.Roles
{
    public class GetAllRolesOrderedByNameUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public GetAllRolesOrderedByNameUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Response>> ExecuteAsync()
        {
            var roles = await _roleRepository.GetAllOrderedByNameAsync();

            return roles.Select(role => new Response(role.Id, role.Name));
        }
    }
}
