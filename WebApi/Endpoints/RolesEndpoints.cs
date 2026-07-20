using Application.Exceptions;
using Application.UseCase.Roles;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.RoleDtos;

namespace WebApi.Endpoints
{
    public static class RolesEndpoints
    {
        public static void MapRolesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/roles")
                .WithTags("Roles")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador" });

            group.MapGet("/{id:int}", async Task<Results<Ok<Response>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetRoleByIdUseCase useCase) =>
            {
                try
                {
                    var role = await useCase.ExecuteAsync(id);
                    return TypedResults.Ok(role);
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
            .WithName("GetRoleById")
            .WithSummary("Get a specific role by its unique identifier");

            group.MapPost("/", async Task<Results<Created<Response>, BadRequest<object>, ProblemHttpResult>> ([FromBody] CreateRequest request, CreateRoleUseCase useCase) =>
            {
                try
                {
                    var createdRole = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/roles/{createdRole.Id}", createdRole);
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
            .WithName("CreateRole")
            .WithSummary("Create a new system role");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<Response>>, ProblemHttpResult>> (
                GetAllRolesOrderedByNameUseCase useCase) =>
            {
                try
                {
                    var roles = await useCase.ExecuteAsync();
                    return TypedResults.Ok(roles);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllRoles")
            .WithSummary("Get all registered roles ordered alphabetically by name");
        }
    }
}