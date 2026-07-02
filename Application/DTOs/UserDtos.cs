
namespace Application.DTOs
{
    public static class UserDtos
    {
        public record UserCreateRequest(
            string Name, 
            string Email, 
            string Password, 
            int RoleId);

        public record UserUpdateProfileRequest(
            int Id, 
            string Name, 
            string Email, 
            int RoleId);

        public record UserChangePasswordRequest(
            int Id, 
            string NewPassword);

        public record LoginRequest(
            string Email, 
            string Password);

        public record UserResponse(
            int Id,
            string Name,
            string Email,
            bool IsActive,
            int RoleId,
            string RoleName);

        public record LoginResponse(
            string Token, 
            UserResponse User);

    }
}
