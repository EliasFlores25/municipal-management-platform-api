
namespace Application.DTOs
{
    public static class EmployeeDtos
    {
        public record EmployeeCreateRequest(
            string Code,
            string FirstName,
            string LastName,
            int PositionId,
            int MunicipalityId);

        public record EmployeeUpdateProfileRequest(
            int Id,
            string FirstName,
            string LastName,
            int PositionId,
            int MunicipalityId);

        public record EmployeeTerminateRequest(int Id, DateTime? ExitDate);

        public record EmployeeReactivateRequest(
            int Id,
            int PositionId,
            int MunicipalityId,
            DateTime? NewContractDate);

        public record EmployeeResponse(
            int Id,
            string Code,
            string FirstName,
            string LastName,
            string FullName,
            DateTime ContractDate,
            DateTime? ExitDate,
            string State,
            int PositionId,
            string PositionName,
            int MunicipalityId,
            string MunicipalityName,
            TimeSpan? ContractDuration);
    }
}
