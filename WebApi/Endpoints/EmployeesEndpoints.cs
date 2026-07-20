using Application.Exceptions;
using Application.UseCases.Employees;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.EmployeeDtos;

namespace WebApi.Endpoints
{
    public static class EmployeesEndpoints
    {
        public static void MapEmployeesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/employees")
                .WithTags("Employees")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador,Director de Operaciones,RRHH" });

            group.MapGet("/", async Task<Results<Ok<IEnumerable<EmployeeResponse>>, ProblemHttpResult>> (
                GetEmployeeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var employees = await queriesUseCase.GetAllAsync();
                    return TypedResults.Ok(employees);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllEmployees")
            .WithSummary("Retrieve all registered corporate employees");

            group.MapGet("/{id:int}", async Task<Results<Ok<EmployeeResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetEmployeeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var employee = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(employee);
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetEmployeeById")
            .WithSummary("Get an individual employee profile using its unique identifier");

            group.MapGet("/code/{code}", async Task<Results<Ok<EmployeeResponse>, NotFound<object>, ProblemHttpResult>> (
                string code,
                GetEmployeeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var employee = await queriesUseCase.GetByCodeAsync(code);
                    return TypedResults.Ok(employee);
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetEmployeeByCode")
            .WithSummary("Search for an active employee record utilizing their custom business code");

            group.MapGet("/status/{status}", async Task<Results<Ok<IEnumerable<EmployeeResponse>>, ProblemHttpResult>> (
                EmployeeStatus status,
                GetEmployeeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var employees = await queriesUseCase.GetByStatusAsync(status);
                    return TypedResults.Ok(employees);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetEmployeesByStatus")
            .WithSummary("Filter employee records by active or inactive states");

            group.MapGet("/position/{positionId:int}", async Task<Results<Ok<IEnumerable<EmployeeResponse>>, ProblemHttpResult>> (
                int positionId,
                GetEmployeeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var employees = await queriesUseCase.GetByPositionAsync(positionId);
                    return TypedResults.Ok(employees);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetEmployeesByPosition")
            .WithSummary("Get a list of employees filtered by their specific corporate position");

            group.MapGet("/municipality/{municipalityId:int}", async Task<Results<Ok<IEnumerable<EmployeeResponse>>, ProblemHttpResult>> (
                int municipalityId,
                GetEmployeeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var employees = await queriesUseCase.GetByMunicipalityAsync(municipalityId);
                    return TypedResults.Ok(employees);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetEmployeesByMunicipality")
            .WithSummary("Group and retrieve staff records matching a geographical municipality assignment");

            group.MapPost("/", async Task<Results<Created<EmployeeResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] EmployeeCreateRequest request,
                CreateEmployeeUseCase useCase) =>
            {
                try
                {
                    var createdEmployee = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/employees/{createdEmployee.Id}", createdEmployee);
                }
                catch (Exception ex) when (ex is ApplicationValidationException || ex is DomainException)
                {
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("CreateEmployee")
            .WithSummary("Register a new staff member within the system ecosystem");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] EmployeeUpdateProfileRequest request,
                UpdateEmployeeProfileUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Los datos del empleado han sido actualizados correctamente." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (DomainException ex)
                {
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("UpdateEmployeeProfile")
            .WithSummary("Modify personal details, structural position, and regional assignment of an employee");

            group.MapPatch("/{id:int}/terminate", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] EmployeeTerminateRequest request,
                EmployeeLifecycleUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID de la solicitud de finalización." });
                }

                try
                {
                    await useCase.TerminateAsync(request);
                    return TypedResults.Ok((object)new { message = "La relación laboral del empleado ha sido finalizada exitosamente." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (DomainException ex)
                {
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("TerminateEmployee")
            .WithSummary("Process the contract separation and exit strategy for a staff worker");

            group.MapPatch("/{id:int}/reactivate", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] EmployeeReactivateRequest request,
                EmployeeLifecycleUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID de la solicitud de reactivación." });
                }

                try
                {
                    await useCase.ReactivateAsync(request);
                    return TypedResults.Ok((object)new { message = "El empleado ha sido reincorporado a la compañía con un nuevo contrato laboral." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (DomainException ex)
                {
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("ReactivateEmployee")
            .WithSummary("Rehire an inactive worker assigning them new contract conditions");
        }
    }
}